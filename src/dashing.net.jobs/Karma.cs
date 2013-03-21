using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dashing.net.common;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    [Export(typeof(IJob))]
    public class Karma : IJob
    {
        private readonly Random _rand;

        public int CurrentKarma { get; private set; }
        public int LastKarma { get; private set; }

        public Lazy<Timer> Timer { get; private set; }

        public Karma()
        {
            _rand = new Random();

            CurrentKarma = _rand.Next(200000);

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)));
        }

        public void SendMessage(object sent)
        {
            LastKarma = CurrentKarma;

            CurrentKarma = _rand.Next(200000);

            Dashing.SendMessage(new {current = CurrentKarma, last = LastKarma, id = "karma"});
        }
    }
}
