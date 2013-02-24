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
using dashing.net.jobs;

namespace dashing.net.Controllers
{
    public class EventsController : ApiController
    {
        //private static StreamWriter _streamWriter;
        private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();
        private static readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>(); 

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            //Timer t = _timer.Value;
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(WriteToStream, "text/event-stream");

            var sample = new Sample(AddToQueue);
            var karma = new Karma(AddToQueue);
            var synergy = new Synergy(AddToQueue);

            return response;
        }

        private void WriteToStream(Stream outputStream, HttpContent headers, TransportContext context)
        {
            var streamWriter = new StreamWriter(outputStream);
            _streammessage.Enqueue(streamWriter);
        }

        private async void AddToQueue(string message)
        {
            _messages.Enqueue(message);

            if (_streammessage.Count == 0)
            {
                return;
            }

            for (int x = 0; x < _messages.Count; x++)
            {
                string data = string.Empty;
                _messages.TryDequeue(out data);

                StreamWriter streamWriter;
                _streammessage.TryDequeue(out streamWriter);

                if (streamWriter != null)
                {
                    await streamWriter.WriteLineAsync(data);
                    
                    try
                    {
                        await streamWriter.FlushAsync();
                        _streammessage.Enqueue(streamWriter);
                    }
                    catch (Exception ex)
                    {
                        // dont re-add the stream as an error ocurred presumable the client has lost connection
                    }
                }
            }
        }
    }
}
