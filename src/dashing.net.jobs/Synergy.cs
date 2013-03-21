using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dashing.net.common;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    [Export(typeof(IJob))]
    public class Synergy : IJob
    {
        private readonly Random _rand;

        public Lazy<Timer> Timer { get; private set; }

        public Synergy()
        {
            _rand = new Random();

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)));
        }

        protected void SendMessage(object message)
        {
            Dashing.SendMessage(new {value = _rand.Next(100), id = "synergy"});
        }
    }
}
