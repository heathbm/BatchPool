namespace BatchPool
{
    public static class BatchPoolExtensions
    {
        /// <summary>
        /// Wait for all tasks that exist in the BatchPoolContainer at the time of calling this method.
        /// </summary>
        /// <param name="pendingTasks">Tasks to wait for to finish.</param>
        public static async Task WaitForAllAsync(this IEnumerable<BatchPoolTask> pendingTasks)
        {
            await BatchPoolContainerBase
                .WaitForAllAsync(pendingTasks)
                .ConfigureAwait(false);
        }
    }
}
