using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class LockfreeQueue<T>
    {
        internal readonly static TimeSpan MinWaitTime = TimeSpan.FromMilliseconds(1);
        internal readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        internal readonly ManualResetEventSlim _event = new ManualResetEventSlim(false);

        public void Push(T data)
        {
            this._queue.Enqueue(data);
            this._event.Set();
        }

        public bool TryPop(out T data, TimeSpan timeSpan)
        {
            if (this._queue.TryDequeue(out data))
                return true;

            var stopWatch = Stopwatch.StartNew();
            while (stopWatch.Elapsed < timeSpan)
            {
                if (this._queue.TryDequeue(out data))
                    return true;

                var remainTime = (timeSpan - stopWatch.Elapsed);

                //남은 대기 시간이 없을 경우 
                if (remainTime <= TimeSpan.Zero)
                    break;
                // 최소 대기 시간보다 작은 경우 
                else if (remainTime < MinWaitTime)
                    remainTime = MinWaitTime;
                
                // remainTime동안 기다리면서 이벤트가 발생한 경우
                if (this._event.Wait(remainTime) == true)
                    _event.Reset(); 
            }

            return false;
        }
    }
}
