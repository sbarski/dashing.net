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
using Quartz;
using Quartz.Impl;

namespace dashing.net.Controllers
{
    public class EventsController : ApiController
    {
        //private static StreamWriter _streamWriter;
        private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();
        private static readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();

        private static Sample sample = null;
        private static Buzzwords buzzwords = null; 

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            //IScheduler sched = schedFact.GetScheduler();
            //sched.Start();

            //Timer t = _timer.Value;
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(WriteToStream, "text/event-stream");

            if (sample == null)
            {
                sample = new Sample(AddToQueue);
               
                //var trigger = new Quartz.Impl.Triggers.SimpleTriggerImpl("Buzzwords", DateTime.UtcNow, null, Quartz.Impl.Triggers.SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromSeconds(sample.Period));
                
                //var jobDetail = new Quartz.Impl.JobDetailImpl("Buzzwords", typeof(Buzzwords));
                
            }
                
            var karma = new Karma(AddToQueue);
            var synergy = new Synergy(AddToQueue);
            var convergence = new Convergence(AddToQueue);

            if (buzzwords == null)
            {
                buzzwords = new Buzzwords(AddToQueue);
            }

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
