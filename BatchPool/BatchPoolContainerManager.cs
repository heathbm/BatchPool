namespace BatchPool
{
    /// <summary>
    /// A manager of a collection of <see cref="BatchPoolContainer"/>.
    /// This class can be used with Dependency Injection (see README for more information).
    /// </summary>
    public class BatchPoolContainerManager<T>
        where T : BatchPoolContainerBase
    {
        private readonly Dictionary<string, T> _batchPoolContainers;

        /// <summary>
        /// Create a BatchPoolContainerManager, that can contain multiple BatchPools.
        /// </summary>
        public BatchPoolContainerManager()
        {
            _batchPoolContainers = new();
        }

        /// <summary>
        /// Create a BatchPoolContainer.
        /// </summary>
        /// <param name="uniqueName">A unique name that will be used for retrieval. Duplicate names cannot be used.</param>
        /// <param name="newBatchPoolContainer">The BatchPoolContainer that will be registered. BatchPoolContainers cannot be registered more than once.</param>
        public T RegisterBatchPool(string uniqueName, T newBatchPoolContainer)
        {
            if (_batchPoolContainers.TryGetValue(uniqueName, out T? value))
            {
                return value;
            }

            _batchPoolContainers.Add(uniqueName, newBatchPoolContainer);

            return newBatchPoolContainer;
        }

        /// <summary>
        /// Try to retrieve an existing BatchPoolContainer by name. Returns null and false if not found.
        /// </summary>
        /// <param name="name">The unique name of the BatchPoolContainer that will be attempted to be retrieved.</param>
        /// <param name="batchPool">The potentially retrieved BatchPoolContainer.</param>
        public bool TryGetBatchPool(string name, out T? batchPool)
        {
            if (_batchPoolContainers.ContainsKey(name))
            {
                batchPool = _batchPoolContainers[name];
                return true;
            }

            batchPool = null;
            return false;
        }

        /// <summary>
        /// Wait for all tasks in all BatchPools owned by this BatchPoolContainerManager at the time of calling this method.
        /// </summary>
        public async Task WaitForAllBatchPools()
        {
            var batchPools = _batchPoolContainers.Values;

            foreach (T batchPool in batchPools)
            {
                await batchPool
                    .WaitForAllAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}
