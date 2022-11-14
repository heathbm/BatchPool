using BatchPool.Tasks.Callbacks;
using BatchPool.Tasks.Containers;

namespace BatchPool.Tasks.BatchTasks
{
    /// <inheritdoc/>
    public class PoolTaskBatchPoolTask : BatchPoolTask
    {
        TaskContainer _taskContainer;

        internal PoolTaskBatchPoolTask(Task task, ICallback? callback)
            : base(callback)
        {
            _taskContainer = new(task);
        }

        /// <inheritdoc/>
        public override bool IsCompleted => 
            !IsCancelled 
            && IsTaskCompleted();

        /// <inheritdoc/>
        public override bool IsCancelled => 
            _taskContainer.Task == null;

        /// <inheritdoc/>
        public override async Task WaitForTaskAsync()
        {
            if (IsCancelled)
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
                if (IsCancelled)
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
            if (IsCancelled)
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
