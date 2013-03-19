using System;
using System.Threading;

namespace dashing.net.common
{
    public interface IJob
    {
        Lazy<Timer> Timer { get; }
    }
}
