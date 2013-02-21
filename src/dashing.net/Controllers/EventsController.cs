using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dashing.net.Controllers
{
    public class EventsController : ApiController
    {
        private static readonly Lazy<Timer> _timer = new Lazy<Timer>(() => new Timer(TimerCallback, null, 0, 1000));
        private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            Timer t = _timer.Value;
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(WriteToStream, "text/event-stream");
            return response;
        }

        private void WriteToStream(Stream outputStream, HttpContent headers, TransportContext context)
        {
            var streamWriter = new StreamWriter(outputStream);
            _streammessage.Enqueue(streamWriter);
        }

        private async static void TimerCallback(object state)
        {

            StreamWriter data;
            Random randNum = new Random();

            for (int x = 0; x < _streammessage.Count; x++)
            {
                _streammessage.TryDequeue(out data);
                await data.WriteLineAsync(latestEvents());
                try
                {
                    await data.FlushAsync();
                    _streammessage.Enqueue(data);
                }
                catch (Exception ex)
                {
                    // dont re-add the stream as an error ocurred presumable the client has lost connection
                }
            }
            //To set timer with random interval
            _timer.Value.Change(TimeSpan.FromMilliseconds(randNum.Next(1, 3) * 500), TimeSpan.FromMilliseconds(-1));

        }

        private static string latestEvents()
        {
            var data = JsonConvert.SerializeObject(new {id = "0", updatedAt = DateTime.UtcNow});
            return string.Format("data: {0}\n\n", data);
        }
    }
}
