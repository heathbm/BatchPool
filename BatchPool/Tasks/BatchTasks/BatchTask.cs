namespace BatchPool
{
    /// <inheritdoc/>
    public class BatchTask : BatchPoolTask
    {
        TaskContainer _taskContainer;

        internal BatchTask(Task task, ICallback? callback)
            : base(callback)
        {
            _taskContainer = new(task);
        }

        /// <inheritdoc/>
        public override bool IsCompleted => 
            !IsCanceled 
            && IsTaskCompleted();

        /// <inheritdoc/>
        public override bool IsCanceled => 
            _taskContainer.Task == null;

        /// <inheritdoc/>
        public override async Task WaitForTaskAsync()
        {
            if (IsCanceled)
            {
                return;
            }

            await WaitForTaskAndCallbackAsync(_taskContainer.Task)
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public override bool Cancel()
        {
            lock (this)
            {
                if (IsCanceled)
                {
                    return true;
                }

                bool isTaskActive = _taskContainer.Task != null
                    && !TaskUtil.IsTaskRunningOrCompleted(_taskContainer.Task);

                if (!isTaskActive)
                {
                    return false;
                }
                HandleCancellation();
                return true;
            }
        }

        /// <inheritdoc/>
        internal override async Task StartAndWaitAsync()
        {
            if (IsCanceled)
            {
                return;
            }

            await StartTaskAndCallBackAndWaitAsync(_taskContainer.Task)
                .ConfigureAwait(false);
        }

        private protected override bool IsTaskCompleted() => 
            _taskContainer.Task?.IsCompleted ?? false;

        private void HandleCancellation() => 
            _taskContainer.Task = null;
    }
}
