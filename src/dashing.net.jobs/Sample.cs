using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using dashing.net.common;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    [Export(typeof(IJob))]
    public class Sample : IJob
    {
        private readonly Random _rand;

        public int CurrentValuation { get; private set; }
        public int LastValuation { get; private set; }
        
        public Lazy<Timer> Timer { get; private set; }

        public Sample()
        {
            _rand = new Random();

            CurrentValuation = _rand.Next(100);

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)));

            var start = Timer.Value;
        }

        protected void SendMessage(object message)
        {
            LastValuation = CurrentValuation;
            
            CurrentValuation = _rand.Next(100);

            Dashing.SendMessage(new {current = CurrentValuation, last = LastValuation, id = "sample"});
        }
    }
}
