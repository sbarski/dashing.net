using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json;

namespace dashing.net.Controllers
{
    public class WidgetsController : ApiController
    {
        public void Post(HttpRequestMessage request)
        {
            var result = request.Content.ReadAsByteArrayAsync().Result;

            var str = System.Text.Encoding.Default.GetString(result);
        }
    }
}
