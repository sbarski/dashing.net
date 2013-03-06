using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using dashing.net.Infrastructure;
using dashing.net.streaming;
using System.Dynamic;

namespace dashing.net.Controllers
{
    public class WidgetsController : ApiController
    {
        public void Post(HttpRequestMessage request)
        {
            var body = request.Content.ReadAsStringAsync().Result;

            var widget = request.RequestUri.Segments[request.RequestUri.Segments.Length - 1];

            var result = JsonConvert.DeserializeObject<dynamic>(body);
            result.id = widget;

            if (Dashing.SendMessage != null)
            {
                Dashing.SendMessage(result);
            }
        }
    }
}
