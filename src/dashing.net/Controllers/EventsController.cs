using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Mvc.Async;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using dashing.net.Infrastructure;
using dashing.net.jobs;
using dashing.net.streaming;

namespace dashing.net.Controllers
{
    public class EventsController : ApiController
    {
        private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();
        private static readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

        private static Sample _sample = null;
        private static Buzzwords _buzzwords = null;
        private static Karma _karma = null;
        private static Synergy _synergy = null;
        private static Convergence _convergence = null;
        private static Twitter _twitter = null;

        /// <summary>
        /// Inspiration http://techbrij.com/real-time-chart-html5-push-sse-asp-net-web-api
        /// </summary>
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(WriteToStream, "text/event-stream");

            LoadJobs();

            return response;
        }

        private void WriteToStream(Stream outputStream, HttpContent headers, TransportContext context)
        {
            var streamWriter = new StreamWriter(outputStream);
            _streammessage.Enqueue(streamWriter);
        }

        private void SendMessage(dynamic message)
        {
            var updatedAt = TimeHelper.ElapsedTimeSinceEpoch();

            if (message.GetType() == typeof(JObject))
            {
                message.updatedAt = updatedAt;
            }
            else
            {
                message = JsonHelper.Merge(message, new { updatedAt });
            }

            ProcessMessage(JsonConvert.SerializeObject(message));
        }

        private static void ProcessMessage(string data)
        {
            var currentMessage = string.Format("data: {0}\n\n", data);

            ProcessQueue(currentMessage);
        }

        private static void ProcessQueue(string message)
        {
            _messageQueue.Enqueue(message);

            for (int x = 0; x < _messageQueue.Count; x++)
            {
                var data = string.Empty;
                _messageQueue.TryPeek(out data);

                StreamWriter streamWriter;
                _streammessage.TryDequeue(out streamWriter);

                if (streamWriter != null && !string.IsNullOrEmpty(data))
                {
                    try
                    {
                        streamWriter.WriteLine(data);
                    }
                    catch (HttpException) //connection was most likely closed
                    {
                        return;
                    }

                    try
                    {
                        streamWriter.Flush();

                        _messageQueue.TryDequeue(out data);
                        
                        _streammessage.Enqueue(streamWriter);
                    }
                    catch (Exception)
                    {
                        // dont re-add the stream as an error ocurred presumable the client has lost connection
                    }
                }
            }
        }

        private void LoadJobs()
        {
            Dashing.SendMessage = SendMessage;

            if (_sample == null)
            {
                _sample = new Sample();
            }

            if (_karma == null)
            {
                _karma = new Karma();
            }

            if (_synergy == null)
            {
                _synergy = new Synergy();
            }

            if (_convergence == null)
            {
                _convergence = new Convergence();
            }

            if (_buzzwords == null)
            {
                _buzzwords = new Buzzwords();
            }

            if (_twitter == null)
            {
                _twitter = new Twitter();
            }
        }
    }
}
