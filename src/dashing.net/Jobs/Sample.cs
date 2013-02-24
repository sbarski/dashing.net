using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dashing.net.Jobs
{
    public class Sample
    {
        private static Random rand;

        public int ScheduleTimeout { get; set; } //in seconds

        public int CurrentValuation { get; private set; }
        public int CurrentKarma { get; private set; }

        public int LastValuation { get; private set; }
        public int LastKarma { get; private set; }

        public Sample()
        {
            rand = new Random();

            CurrentValuation = rand.Next(100);
            CurrentKarma = rand.Next(200000);

            ScheduleTimeout = 2;
        }

        public override string ToString()
        {
            rand = new Random();

            LastValuation = CurrentValuation;
            LastKarma = CurrentKarma;

            CurrentValuation = rand.Next(100);
            CurrentKarma = rand.Next(200000);

            return "";
        }
    }
}