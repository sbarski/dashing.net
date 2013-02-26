using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashing.net.jobs
{
    public class Convergence : Job
    {
        private Random _rand;

        private ConcurrentQueue<Tuple<int, int>> _points; 
        private int _lastX;

        public Convergence(Action<string> sendMessage)
            : base(sendMessage, "convergence", TimeSpan.FromSeconds(2))
        {
            _rand = new Random();
            _points = new ConcurrentQueue<Tuple<int, int>>();

            for (int i = 0; i < 10; i++)
            {
                _points.Enqueue(new Tuple<int, int>(i, _rand.Next(50)));
            }

            _lastX = _points.Count - 1;
    
            Start();
        }

        protected override object GetData()
        {
            Tuple<int,int> obj;
            
            _lastX += 1;

            _points.TryDequeue(out obj);

            _points.Enqueue(new Tuple<int, int>(_lastX, _rand.Next(50)));

            return new {points = _points.Select(m => new {x = m.Item1, y = m.Item2})};
        }
    }
}
