using System;
using System.Collections.Generic;
using System.Text;

namespace ServerCore.Utility
{
    public class ObjectPool<T>
    {
        private Stack<T> _objectStack;
        private Func<T> _generator;
        private Action<T> _onAlloc;
        private Action<T> _onFree;
        private int _capacity = 0;

        public int Capacity => _capacity;
        public int AvailableCount => _objectStack.Count;
        public int AllocatedCount => _capacity - _objectStack.Count;

        public ObjectPool(Func<T> generator, int capacity, Action<T> onAlloc = null, Action<T> onFree = null)
        {
            if (generator == null)
            {
                throw new ArgumentNullException("generator is null");
            }

            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException("capacity < 1");
            }

            _objectStack = new Stack<T>(capacity);
            _generator = generator;
            _onAlloc = onAlloc;
            _onFree = onFree;
            _capacity = capacity;

            for (int i = 0; i < _capacity; ++i)
            {
                _objectStack.Push(_generator());
            }
        }

        public T Pop()
        {
            if (_objectStack.Count > 0)
            {
                var obj = _objectStack.Pop();
                _onAlloc?.Invoke(obj);
                return obj;
            }

            var newObj = _generator();
            _onAlloc?.Invoke(newObj);
            this._capacity++;
            return newObj;
        }

        public void Push(T obj)
        {
            _onFree?.Invoke(obj);
            _objectStack.Push(obj);
        }

        public void Resize(int newCapacity)
        {
            if (_capacity >= newCapacity)
            {
                return;
            }

            for (int count = _capacity; count < newCapacity; ++count)
            {
                _objectStack.Push(_generator());
            }

            _capacity = newCapacity;
        }
    }
}
