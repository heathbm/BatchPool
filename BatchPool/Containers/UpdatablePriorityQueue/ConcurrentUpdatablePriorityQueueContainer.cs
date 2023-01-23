using System.Runtime.CompilerServices;
using BatchPool.Containers;

namespace BatchPool
{
    internal class ConcurrentUpdatablePriorityQueueContainer<T> : IBatchTaskQueueContainer
        where T : IComparable<T>
    {
        private readonly PriorityQueue<BatchPoolTask, PriorityStateContainer<T>> _queue;
        private readonly Dictionary<BatchPoolTask, PriorityStateContainer<T>> _activePriorities;

        internal ConcurrentUpdatablePriorityQueueContainer()
        {
            _queue = new();
            _activePriorities = new();
        }

        internal ConcurrentUpdatablePriorityQueueContainer(IComparer<T> comparer)
        {
            _queue = new(new PriorityStateContainerComparer<T>(comparer));
            _activePriorities = new();
        }

        public bool IsEmpty =>
            _queue.Count == 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetPendingTaskCount() =>
            _activePriorities
            .Keys
            .Count(a => !a.IsCanceled);

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
                PriorityStateContainer<T> priorityStateContainer = new(priority);
                _queue.Enqueue(batchPoolTask, priorityStateContainer);
                _activePriorities.Add(batchPoolTask, priorityStateContainer);
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
                while (_queue.TryDequeue(out batchPoolTask, out PriorityStateContainer<T>? priorityStateContainer))
                {
                    _activePriorities.Remove(batchPoolTask);

                    if (!priorityStateContainer.IsDisabled)
                    {
                        value = priorityStateContainer.Priority;
                        return true;
                    }
                }

                batchPoolTask = null;
                value = default;
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool UpdatePriority(BatchPoolTask batchPoolTask, T newPriority)
        {
            // Instead of modifying the existing Min-Heap,
            // we flag the previous node as invalid and add a new one
            // This is better for perf with large PriorityQueue size or frequent priority updates
            lock (_queue)
            {
                if (!_activePriorities.TryGetValue(batchPoolTask, out PriorityStateContainer<T>? priorityStateContainer))
                {
                    // The requested update is not associated with an existing BatchTask
                    return false;
                }

                if (_activePriorities[batchPoolTask].Priority.CompareTo(newPriority) == 0)
                {
                    // Same priority, return
                    return false;
                }

                // Disable previous priority
                priorityStateContainer.Disable();

                // Create new priority and associate with existing BatchTask
                PriorityStateContainer<T> newPriorityStateContainer = new(newPriority);
                _queue.Enqueue(batchPoolTask, newPriorityStateContainer);
                _activePriorities[batchPoolTask] = newPriorityStateContainer;

                return true;
            }
        }
    }
}