namespace BatchPool
{
    /// <summary>
    /// A container for a task and an optional Callback.
    /// </summary>
    public abstract class BatchPoolTask
    {
        private readonly ICallback? _callback;

        private protected BatchPoolTask(ICallback? callback)
        {
            _callback = callback;
        }

        /// <summary>
        /// Wait for the task to complete.
        /// </summary>
        public abstract Task WaitForTaskAsync();

        /// <summary>
        /// Cancel the task. Returns true if the task was canceled successfully.
        /// </summary>
        public abstract bool Cancel();

        /// <summary>
        /// Has the task completed without being canceled.
        /// </summary>
        public abstract bool IsCompleted { get; }

        /// <summary>
        /// Is the Task permanently canceled.
        /// </summary>
        public abstract bool IsCanceled { get; }

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

                if (_callback != null)
                {
                    await _callback
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

            if (_callback != null)
            {
                await _callback
                    .WaitForCallbackIfRequired()
                    .ConfigureAwait(false);
            }
        }
    }
}