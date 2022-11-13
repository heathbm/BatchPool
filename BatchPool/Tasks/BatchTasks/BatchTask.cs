using BatchPool.Tasks.Callbacks;

namespace BatchPool.Tasks.BatchTasks
{
    /// <summary>
    /// A container for a task and Callback.
    /// </summary>
    public abstract class BatchTask
    {
        private readonly ICallback? _batchTaskCallback;

        private protected BatchTask(ICallback? batchTaskCallback)
        {
            _batchTaskCallback = batchTaskCallback;
        }

        /// <summary>
        /// Wait for the task to complete.
        /// </summary>
        public abstract Task WaitForTaskAsync();

        /// <summary>
        /// Cancel the task. Returns true if the task was cancelled successfully.
        /// </summary>
        public abstract bool Cancel();

        /// <summary>
        /// Has the task completed without being cancelled.
        /// </summary>
        public abstract bool IsCompleted { get; }

        /// <summary>
        /// Is the Task permanently cancelled.
        /// </summary>
        public abstract bool IsCancelled { get; }

        /// <summary>
        /// Start the task and wait for the task to complete.
        /// </summary>
        internal abstract Task StartAndWaitAsync();

        private protected abstract bool IsTaskCompleted();

        private protected async Task StartTaskAndCallBackAndWaitAsync(Task? task, bool awaitTask = false)
        {
            if (task == null)
            {
                return;
            }

            if (!TaskUtil.IsTaskRunningOrCompleted(task))
            {
                task.Start();
                awaitTask = true;
            }

            if (awaitTask)
            {
                await task.ConfigureAwait(false);

                if (_batchTaskCallback != null)
                {
                    await _batchTaskCallback
                        .RunCallbackIfPresent()
                        .ConfigureAwait(false);
                }
            }
        }

        private protected async Task WaitForTaskAndCallbackAsync(Task? task)
        {
            if (task != null)
            {
                await task.ConfigureAwait(false);
            }

            if (_batchTaskCallback != null)
            {
                await _batchTaskCallback
                    .WaitForCallbackIfRequired()
                    .ConfigureAwait(false);
            }
        }
    }
}