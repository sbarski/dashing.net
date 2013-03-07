using System;
using System.Collections.Generic;
using System.Configuration;
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
        public HttpResponseMessage Post(HttpRequestMessage request)
        {
            var body = request.Content.ReadAsStringAsync().Result;

            var widget = request.RequestUri.Segments[request.RequestUri.Segments.Length - 1];

            dynamic result;

            try
            {
                result = JsonConvert.DeserializeObject<dynamic>(body);
            }
            catch (JsonReaderException)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            var authTokenInMessage = result["auth_token"] != null ? result["auth_token"].Value : null;
            
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AuthToken"]) ||
                string.Equals(ConfigurationManager.AppSettings["AuthToken"], authTokenInMessage))
            {
                result.id = widget;

                if (Dashing.SendMessage != null)
                {
                    Dashing.SendMessage(result);
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            
            return new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}
