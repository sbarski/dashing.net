using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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
using dashing.net.common;
using dashing.net.jobs;
using dashing.net.streaming;

namespace dashing.net.Controllers
{
    public class EventsController : ApiController
    {
        private static readonly ConcurrentBag<StreamWriter> _streammessage = new ConcurrentBag<StreamWriter>();
        private static readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();

        private static readonly IList<IJob> _jobs = new List<IJob>();

        private static bool _hasInstantiated = false;

        /// <summary>
        /// Inspiration http://techbrij.com/real-time-chart-html5-push-sse-asp-net-web-api
        /// </summary>
        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(WriteToStream, "text/event-stream");

            if (!_hasInstantiated)
            {
                LoadJobs();

                _hasInstantiated = true;
            }

            return response;
        }

        private void WriteToStream(Stream outputStream, HttpContent headers, TransportContext context)
        {
            var streamWriter = new StreamWriter(outputStream);
            _streammessage.Add(streamWriter);
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
                string data;
                _messageQueue.TryPeek(out data);

                for (int i = 0; i < _streammessage.Count; i++)
                {
                    StreamWriter streamWriter;

                    if (!string.IsNullOrEmpty(data) && _streammessage.TryTake(out streamWriter))
                    {
                        try
                        {
                            streamWriter.WriteLine(data);
                        }
                        catch (HttpException) //connection was most likely closed
                        {
                            break;
                        }

                        try
                        {
                            streamWriter.Flush();

                            _messageQueue.TryDequeue(out data);
                        }
                        catch (Exception)
                        {
                            // dont re-add the stream as an error ocurred presumable the client has lost connection
                            break;
                        }

                        _streammessage.Add(streamWriter);
                    }
                }
            }
        }

        private void LoadJobs()
        {
            Dashing.SendMessage = SendMessage;

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(HttpContext.Current.Server.MapPath("~/Jobs/")));
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));

            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            var exports = container.GetExportedValues<IJob>();

            foreach (var job in exports)
            {
                _jobs.Add(job);
            }
        }
    }
}
