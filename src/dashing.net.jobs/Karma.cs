using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashing.net.jobs
{
    public class Karma : Job
    {
        private static Random _rand;

        public int CurrentKarma { get; private set; }
        public int LastKarma { get; private set; }

        public Karma(Action<string> sendMessage)
            : base(sendMessage, "karma", 1000)
        {
            _rand = new Random();

            CurrentKarma = _rand.Next(200000);

            Start();
        }

        protected override object GetData()
        {
            LastKarma = CurrentKarma;

            CurrentKarma = _rand.Next(200000);

            return new { current = CurrentKarma, last = LastKarma };
        }
    }
}
