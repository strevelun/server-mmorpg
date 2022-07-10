using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ServerCore
{
    public abstract class Worker
    {
        protected ConcurrentQueue<Producer> _producers = new ConcurrentQueue<Producer>();

        public void BindProducer(Producer producer)
        {
			_producers.Enqueue(producer);
        }

        public Producer Pop()
        {
            return _producers.TryDequeue(out Producer producer) == true ? producer : null;
        }

        public abstract void Execute();
    }
}
