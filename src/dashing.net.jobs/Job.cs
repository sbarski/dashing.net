using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dashing.net.jobs
{
    public abstract class Job
    {
        private readonly Lazy<Timer> _timer;
        private readonly string _id;

        public int Period { get; set; }

        private Action<string> SendMessage { get; set; }

        protected bool IsReady { get; set; }

        protected Job(Action<string> sendMessage, string id, TimeSpan timeSpan)
        {
            _id = id;

            SendMessage = sendMessage;

            _timer = new Lazy<Timer>(() => new Timer(TimerCallback, null, TimeSpan.Zero, timeSpan));
        }

        public void Start()
        {
            Timer t = _timer.Value;
        }

        private void TimerCallback(object state)
        {
            SendMessage(SendEvent(GetData()));
        }

        protected abstract object GetData();

        private string SendEvent(object message)
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;

            var payload = Merge(message, new { id = _id, updatedAt = secondsSinceEpoch });
            
            var data = JsonConvert.SerializeObject(payload);
            return string.Format("data: {0}\n\n", data);
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
