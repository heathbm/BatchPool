using System.Collections.Generic;

namespace BatchPool.UnitTests.Utilities
{
    internal class ExecutionOrderTracker
    {
        private readonly Queue<string> _queue;

        internal ExecutionOrderTracker()
        {
            _queue = new();
        }

        internal void Add(string value)
        {
            lock (_queue)
            {
                _queue.Enqueue(value);
            }
        }

        internal string GetNext()
        {
            lock (_queue)
            {
                return _queue.Dequeue();
            }
        }
    }
}
