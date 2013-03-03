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
using dashing.net.Infrastructure;
using dashing.net.jobs;
using Quartz;
using Quartz.Impl;
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

        private async void SendMessage(object message)
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var secondsSinceEpoch = (int)t.TotalSeconds;

            var payload = JsonHelper.Merge(message, new {updatedAt = secondsSinceEpoch});

            var data = JsonConvert.SerializeObject(payload);

            var currentMessage = string.Format("data: {0}\n\n", data);

            ProcessQueue(currentMessage);

        }

        private static async void ProcessQueue(string message)
        {
            _messageQueue.Enqueue(message);

            for (int x = 0; x < _messageQueue.Count; x++)
            {
                string data = string.Empty;
                _messageQueue.TryDequeue(out data);

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
