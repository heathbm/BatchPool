namespace BatchPool
{
    /// <summary>
    /// A manager of a collection of <see cref="BatchPoolContainer"/>.
    /// This class can be used with Dependency Injection (see README for more information).
    /// </summary>
    public class BatchPoolContainerManager
    {
        private readonly Dictionary<string, BatchPoolContainer> _batchPoolContainers;

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
        /// <param name="batchSize">Max number of tasks that will run concurrently.</param>
        /// <param name="isEnabled">Is the BatchPoolContainer enabled by default. If false, the BatchPoolContainer must be started manually.</param>
        public BatchPoolContainer CreateAndRegisterBatch(string uniqueName, int batchSize, bool isEnabled = true)
        {
            if (_batchPoolContainers.ContainsKey(uniqueName))
            {
                return _batchPoolContainers[uniqueName];
            }

            BatchPoolContainer newBatchPoolContainer = new(batchSize, isEnabled);
            _batchPoolContainers.Add(uniqueName, newBatchPoolContainer);

            return newBatchPoolContainer;
        }

        /// <summary>
        /// Try to retrieve an existing BatchPoolContainer by name. Returns null and false if not found.
        /// </summary>
        /// <param name="name">The unique name of the BatchPoolContainer that will be attempted to be retrieved.</param>
        /// <param name="batchPool">The potentially retrieved BatchPoolContainer.</param>
        public bool TryGetBatchPool(string name, out BatchPoolContainer? batchPool)
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

            foreach (BatchPoolContainer batchPool in batchPools)
            {
                await batchPool
                    .WaitForAllAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}
