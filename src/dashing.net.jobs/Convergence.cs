using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using dashing.net.common;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    [Export(typeof(IJob))]
    public class Convergence : IJob
    {
        private readonly Random _rand;

        private readonly ConcurrentQueue<Tuple<int, int>> _points; 
        private int _lastX;

        public Lazy<Timer> Timer { get; private set; }

        public Convergence()
        {
            _rand = new Random();
            _points = new ConcurrentQueue<Tuple<int, int>>();

            for (int i = 0; i < 10; i++)
            {
                _points.Enqueue(new Tuple<int, int>(i, _rand.Next(50)));
            }

            _lastX = _points.Count - 1;

            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)));

            var start = Timer.Value;
        }

        protected void SendMessage(object message)
        {
            Tuple<int,int> obj;
            
            _lastX += 1;

            _points.TryDequeue(out obj);

            _points.Enqueue(new Tuple<int, int>(_lastX, _rand.Next(50)));

            Dashing.SendMessage(new {id = "convergence", points = _points.Select(m => new {x = m.Item1, y = m.Item2})});
        }

    }
}
