using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace dashing.net.jobs
{
    public class Sample : Job
    {
        private static Random _rand;

        public int CurrentValuation { get; private set; }
        public int LastValuation { get; private set; }
        
        public Sample(Action<string> sendMessage)
            : base(sendMessage, "valuation", 2000)
        {
            _rand = new Random();

            CurrentValuation = _rand.Next(100);

            Start();
        }

        protected override object GetData()
        {
            LastValuation = CurrentValuation;
            
            CurrentValuation = _rand.Next(100);
            
            return new {current = CurrentValuation, last = LastValuation};
        }
    }
}
