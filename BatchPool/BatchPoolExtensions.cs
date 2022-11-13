using BatchPool.Tasks.BatchTasks;

namespace BatchPool
{
    public static class BatchPoolExtensions
    {
        /// <summary>
        /// Wait for all tasks that exist in the BatchPool at the time of calling this method.
        /// </summary>
        /// <param name="pendingTasks">Tasks to wait for to finish.</param>
        public static async Task WaitForAllAsync(this IEnumerable<BatchTask> pendingTasks)
        {
            await BatchPool
                .WaitForAllAsync(pendingTasks)
                .ConfigureAwait(false);
        }
    }
}
