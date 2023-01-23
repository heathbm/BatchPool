using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace BatchPool
{
    /// <summary>
    /// FIFO standard queue logic
    /// </summary>
    internal class ConcurrentQueueContainer : IBatchTaskQueueContainer
    {
        private readonly ConcurrentQueue<BatchPoolTask> _queue;

        internal ConcurrentQueueContainer()
        {
            _queue = new();
        }

        public bool IsEmpty =>
            _queue.IsEmpty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetPendingTaskCount() =>
            _queue.Count(a => !a.IsCanceled);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BatchPoolTask[] ToArray() =>
            _queue.ToArray();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Enqueue(BatchPoolTask batchPoolTask) =>
            _queue.Enqueue(batchPoolTask);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeue(out BatchPoolTask? batchPoolTask) =>
            _queue.TryDequeue(out batchPoolTask);
    }
}