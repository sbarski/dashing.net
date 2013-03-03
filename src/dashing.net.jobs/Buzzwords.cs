using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    public class Buzzwords : IJob
    {
        private readonly Random _rand;
        private readonly string[] _buzzwords =
            {
                "Paradigm shift", "Leverage", "Pivoting", "Turn-key", "Streamlininess",
                "Exit Strategy", "Synergy", "Enterprise", "Web 2.0"
            };

        private Dictionary<string, int> _buzzwordCounts = new Dictionary<string, int>();

        public Lazy<Timer> Timer { get; private set; }

        public Buzzwords()
        {
            _rand = new Random();

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)));

            var start = Timer.Value;
        }

        protected void SendMessage(object message)
        {
            var random = _buzzwords[_rand.Next(_buzzwords.Length)];

            if (_buzzwordCounts.ContainsKey(random))
            {
                _buzzwordCounts[random] = (_buzzwordCounts[random] + 1)%30;
            }
            else
            {
                _buzzwordCounts.Add(random, 1);
            }

            Dashing.SendMessage(new {id = "buzzwords", items = _buzzwordCounts.Select(m => new {label = m.Key, value = m.Value})});
        }
    }
}
