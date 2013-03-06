using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dashing.net.Infrastructure
{
    public static class TimeHelper
    {
        public static int ElapsedTimeSinceEpoch()
        {
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}