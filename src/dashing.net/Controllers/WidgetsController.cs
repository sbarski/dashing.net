using System;
using System.Collections.Generic;
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
using dashing.net.Infrastructure;
using dashing.net.streaming;
using System.Dynamic;

namespace dashing.net.Controllers
{
    public class WidgetsController : ApiController
    {
        public void Post(HttpRequestMessage request)
        {
            var result = request.Content.ReadAsByteArrayAsync().Result;

            var body = System.Text.Encoding.Default.GetString(result);

            var widget = request.RequestUri.Segments[request.RequestUri.Segments.Length - 1];

            var jsonSer = new JavaScriptSerializer();
            var json = jsonSer.Deserialize<object>(body);  //JsonConvert.DeserializeObject(body);

            if (Dashing.SendMessage != null)
            {
                Dashing.SendMessage(JsonHelper.Merge(new { id = widget}, json ));
            }
        }
    }
}
