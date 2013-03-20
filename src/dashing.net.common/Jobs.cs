using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashing.net.common
{
    public static class Jobs
    {
        private static readonly IList<IJob> _jobs = new List<IJob>();

        public static void Add(IJob job)
        {
            _jobs.Add(job);
        }

        public static IEnumerable<IJob> Get()
        {
            return _jobs;
        }
    }
}
