namespace BatchPool
{
    /// <summary>
    /// A BatchPool factory helper to create all available BatchPool types.
    /// </summary>
    public static class BatchPoolFactory
    {
        /// <summary>
        /// A generic task batching and management library.
        /// Tasks are executed in the order in which they are received.
        /// </summary>
        /// <param name="batchSize">Max number of tasks that will run concurrently.</param>
        /// <param name="isEnabled">Is the BatchPoolContainer enabled by default. If false, the BatchPoolContainer must be started manually.</param>
        /// <param name="inactiveTaskValidation">If enabled and a running task is added to the BatchPoolContainer, an Exception will be thrown</param>
        /// <param name="cancellationToken">A Cancellation Token that will permanently cancel the BatchPoolContainer.</param>
        public static BatchPoolContainer GetQueueBatchPool(int batchSize, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
        {
            return new BatchPoolContainer(batchSize, isEnabled, inactiveTaskValidation, cancellationToken);
        }

        /// <summary>
        /// A generic task batching and management library.
        /// Tasks execution order is dynamically provided as tasks are added.
        /// </summary>
        /// <typeparam name="T">Type that will instruct the dynamic ordering.</typeparam>
        /// <param name="batchSize">Max number of tasks that will run concurrently.</param>
        /// <param name="isEnabled">Is the BatchPoolContainer enabled by default. If false, the BatchPoolContainer must be started manually.</param>
        /// <param name="inactiveTaskValidation">If enabled and a running task is added to the BatchPoolContainer, an Exception will be thrown</param>
        /// <param name="cancellationToken">A Cancellation Token that will permanently cancel the BatchPoolContainer.</param>
        public static BatchPoolDynamicContainer<T> GetDynamicallyOrderedBatchPool<T>(int batchSize, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
        {
            return new BatchPoolDynamicContainer<T>(batchSize, isEnabled, inactiveTaskValidation, cancellationToken);
        }

        /// <summary>
        /// A generic task batching and management library.
        /// Tasks execution order is dynamically provided as tasks are added.
        /// </summary>
        /// <typeparam name="T">Type that will instruct the dynamic ordering.</typeparam>
        /// <param name="batchSize">Max number of tasks that will run concurrently.</param>
        /// <param name="comparer">An override comparer that will be used by the internal PriorityQueue (Min-Heap)</param>
        /// <param name="isEnabled">Is the BatchPoolContainer enabled by default. If false, the BatchPoolContainer must be started manually.</param>
        /// <param name="inactiveTaskValidation">If enabled and a running task is added to the BatchPoolContainer, an Exception will be thrown</param>
        /// <param name="cancellationToken">A Cancellation Token that will permanently cancel the BatchPoolContainer.</param>
        public static BatchPoolDynamicContainer<T> GetDynamicallyOrderedBatchPool<T>(int batchSize, IComparer<T> comparer, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
        {
            return new BatchPoolDynamicContainer<T>(batchSize, comparer, isEnabled, inactiveTaskValidation, cancellationToken);
        }

        /// <summary>
        /// A generic task batching and management library.
        /// Tasks execution order is dynamically provided as tasks are added and can be updated later.
        /// </summary>
        /// <typeparam name="T">Type that will instruct the dynamic ordering.</typeparam>
        /// <param name="batchSize">Max number of tasks that will run concurrently.</param>
        /// <param name="isEnabled">Is the BatchPoolContainer enabled by default. If false, the BatchPoolContainer must be started manually.</param>
        /// <param name="inactiveTaskValidation">If enabled and a running task is added to the BatchPoolContainer, an Exception will be thrown</param>
        /// <param name="cancellationToken">A Cancellation Token that will permanently cancel the BatchPoolContainer.</param>
        public static BatchPoolUpdatableDynamicContainer<T> GetUpdatableDynamicallyOrderedBatchPool<T>(int batchSize, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
            where T : IComparable<T>
        {
            return new BatchPoolUpdatableDynamicContainer<T>(batchSize, isEnabled, inactiveTaskValidation, cancellationToken);
        }

        /// <summary>
        /// A generic task batching and management library.
        /// Tasks execution order is dynamically provided as tasks are added and can be updated later.
        /// </summary>
        /// <typeparam name="T">Type that will instruct the dynamic ordering.</typeparam>
        /// <param name="batchSize">Max number of tasks that will run concurrently.</param>
        /// <param name="comparer">An override comparer that will be used by the internal PriorityQueue (Min-Heap)</param>
        /// <param name="isEnabled">Is the BatchPoolContainer enabled by default. If false, the BatchPoolContainer must be started manually.</param>
        /// <param name="inactiveTaskValidation">If enabled and a running task is added to the BatchPoolContainer, an Exception will be thrown</param>
        /// <param name="cancellationToken">A Cancellation Token that will permanently cancel the BatchPoolContainer.</param>
        public static BatchPoolUpdatableDynamicContainer<T> GetUpdatableDynamicallyOrderedBatchPool<T>(int batchSize, IComparer<T> comparer, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
            where T : IComparable<T>
        {
            return new BatchPoolUpdatableDynamicContainer<T>(batchSize, comparer, isEnabled, inactiveTaskValidation, cancellationToken);
        }
    }
}
