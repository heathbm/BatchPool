using System.Runtime.CompilerServices;

namespace BatchPool
{
    internal class ConcurrentPriorityQueueContainer<T> : IBatchTaskQueueContainer
    {
        private readonly PriorityQueue<BatchPoolTask, T> _queue;

        internal ConcurrentPriorityQueueContainer()
        {
            _queue = new();
        }

        internal ConcurrentPriorityQueueContainer(IComparer<T> comparer)
        {
            _queue = new(comparer);
        }

        public bool IsEmpty =>
            _queue.Count == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetPendingTaskCount() =>
            _queue
            .UnorderedItems
            .Count(a => !a.Element.IsCanceled);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BatchPoolTask[] ToArray()
        {
            lock (_queue)
            {
                return _queue
                    .UnorderedItems
                    .Select(a => a.Element)
                    .ToArray();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(BatchPoolTask batchPoolTask, T priority)
        {
            lock (_queue)
            {
                _queue.Enqueue(batchPoolTask, priority);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out BatchPoolTask? batchPoolTask)
        {
            return TryDequeue(out batchPoolTask, out _);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out BatchPoolTask? batchPoolTask, out T? value)
        {
            lock (_queue)
            {
                return _queue.TryDequeue(out batchPoolTask, out value);
            }
        }
    }
}