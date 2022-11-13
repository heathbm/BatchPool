using BatchPool.Tasks.Callbacks;
using BatchPool.Tasks.Containers;

namespace BatchPool.Tasks.BatchTasks
{
    /// <inheritdoc/>
    public class TaskBatchTask : BatchTask
    {
        TaskContainer _batchTaskWithTaskDto;

        internal TaskBatchTask(Task task, ICallback? batchTaskCallback)
            : base(batchTaskCallback)
        {
            _batchTaskWithTaskDto = new(task);
        }

        /// <inheritdoc/>
        public override bool IsCompleted => 
            !IsCancelled 
            && IsTaskCompleted();

        /// <inheritdoc/>
        public override bool IsCancelled => 
            _batchTaskWithTaskDto.Task == null;

        /// <inheritdoc/>
        public override async Task WaitForTaskAsync()
        {
            if (IsCancelled)
            {
                return;
            }

            await WaitForTaskAndCallbackAsync(_batchTaskWithTaskDto.Task)
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

                bool isTaskActive = _batchTaskWithTaskDto.Task != null
                    && !TaskUtil.IsTaskRunningOrCompleted(_batchTaskWithTaskDto.Task);

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

            await StartTaskAndCallBackAndWaitAsync(_batchTaskWithTaskDto.Task)
                .ConfigureAwait(false);
        }

        private protected override bool IsTaskCompleted() => 
            _batchTaskWithTaskDto.Task?.IsCompleted ?? false;

        private void HandleCancellation() => 
            _batchTaskWithTaskDto.Task = null;
    }
}
