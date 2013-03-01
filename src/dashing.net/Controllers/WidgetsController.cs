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
using dashing.net.streaming;
using System.Dynamic;

namespace dashing.net.Controllers
{
    public class WidgetsController : ApiController
    {
        public void Post(HttpRequestMessage request)
        {
            var result = request.Content.ReadAsByteArrayAsync().Result;

            var str = System.Text.Encoding.Default.GetString(result);

            var widget = request.RequestUri.Segments[request.RequestUri.Segments.Length - 1];

            var message = new { text = str };

            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;

            var payload = Merge(message, new { id = widget, updatedAt = secondsSinceEpoch });

            var data = JsonConvert.SerializeObject(payload);
            Streaming.SendMessage(string.Format("data: {0}\n\n", data));
        }

        private dynamic Merge(object item1, object item2)
        {
            if (item1 == null || item2 == null)
            {
                return item1 ?? item2 ?? new ExpandoObject();
            }

            dynamic expando = new ExpandoObject();

            var result = expando as IDictionary<string, object>;

            foreach (System.Reflection.PropertyInfo fi in item1.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item1, null);
            }

            foreach (System.Reflection.PropertyInfo fi in item2.GetType().GetProperties())
            {
                result[fi.Name] = fi.GetValue(item2, null);
            }

            return result;
        }
    }
}
