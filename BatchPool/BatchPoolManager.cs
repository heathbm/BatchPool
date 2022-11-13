namespace BatchPool
{
    public class BatchPoolManager
    {
        private readonly Dictionary<string, BatchPool> _batchPools;

        /// <summary>
        /// Create a BatchPoolManager, that can contain multiple BatchPools.
        /// </summary>
        public BatchPoolManager()
        {
            _batchPools = new();
        }

        /// <summary>
        /// Create a BatchPool.
        /// </summary>
        /// <param name="uniqueName">A unique name that will be used for retrieval. Duplicate names cannot be used.</param>
        /// <param name="batchSize">Max number of tasks that will run concurrently.</param>
        /// <param name="isEnabled">Is the BatchPool enabled by default. If false, the BatchPool must be started manually.</param>
        public BatchPool CreateAndRegisterBatch(string uniqueName, int batchSize, bool isEnabled = true)
        {
            if (_batchPools.ContainsKey(uniqueName))
            {
                return _batchPools[uniqueName];
            }

            BatchPool newBatchPool = new(batchSize, isEnabled);
            _batchPools.Add(uniqueName, newBatchPool);

            return newBatchPool;
        }

        /// <summary>
        /// Try to retrieve an existing BatchPool by name. Returns null and false if not found.
        /// </summary>
        /// <param name="name">The unique name of the BatchPool that will be attempted to be retrieved.</param>
        /// <param name="batchPool">The potentially retrieved BatchPool.</param>
        public bool TryGetBatchPool(string name, out BatchPool? batchPool)
        {
            if (_batchPools.ContainsKey(name))
            {
                batchPool = _batchPools[name];
                return true;
            }

            batchPool = null;
            return false;
        }

        /// <summary>
        /// Wait for all tasks in all BatchPools owned by this BatchPoolManager at the time of calling this method.
        /// </summary>
        public async Task WaitForAllBatchPools()
        {
            var batchPools = _batchPools.Values;

            foreach (BatchPool batchPool in batchPools)
            {
                await batchPool
                    .WaitForAllAsync()
                    .ConfigureAwait(false);
            }
        }
    }
}
