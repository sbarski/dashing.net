using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashing.net.jobs
{
    public class Buzzwords : Job
    {
        private Random _rand;
        private string[] _buzzwords =
            {
                "Paradigm shift", "Leverage", "Pivoting", "Turn-key", "Streamlininess",
                "Exit Strategy", "Synergy", "Enterprise", "Web 2.0"
            };

        private Dictionary<string, int> _buzzwordCounts = new Dictionary<string, int>(); 

        public Buzzwords(Action<string> sendMessage) : base(sendMessage, "buzzwords", 500)
        {
            _rand = new Random();

            Start();
        }

        protected override object GetData()
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

            return new {items = _buzzwordCounts.Select(m => new {label = m.Key, value = m.Value})};
        }
    }
}
