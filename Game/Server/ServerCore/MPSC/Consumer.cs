using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class Consumer
    {
        internal static Consumer _instance = new Consumer();

        internal LockfreeQueue<Worker> _jobQueue = new LockfreeQueue<Worker>();

        internal CancellationTokenSource _cts = new CancellationTokenSource();

        public static Consumer Instance { get { return _instance; } }

        public void ProcessJobQueue()
        {
            var timeSpan = TimeSpan.FromMilliseconds(10); // 최소 대기 시간

            while (!this._cts.IsCancellationRequested)
            {
                if (!this._jobQueue.TryPop(out Worker worker, timeSpan))
                    continue;

                worker.Execute();
            }
        }

        public void PushWorker(Producer producer, Worker worker)
        {
            worker.BindProducer(producer);
            _jobQueue.Push(worker);
        }
    }
}
