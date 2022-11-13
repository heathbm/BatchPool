using BatchPool.Tasks.Callbacks;
using BatchPool.Tasks.Containers;

namespace BatchPool.Tasks.BatchTasks
{
    /// <inheritdoc/>
    internal class FunctionBatchTask : BatchTask
    {
        FunctionContainer _batchTaskWithFunctionDto;

        internal FunctionBatchTask(Func<Task> function, ICallback? batchTaskCallback)
            : base(batchTaskCallback)
        {
            _batchTaskWithFunctionDto = new(function);
        }

        /// <inheritdoc/>
        public override bool IsCompleted => 
            !IsCancelled 
            && IsTaskCompleted();

        /// <inheritdoc/>
        public override bool IsCancelled => 
            _batchTaskWithFunctionDto.Task == null 
            && _batchTaskWithFunctionDto.Function == null;

        /// <inheritdoc/>
        public override async Task WaitForTaskAsync()
        {
            if (IsCancelled)
            {
                return;
            }

            await _batchTaskWithFunctionDto
                .WaitForTaskToComplete()
                .ConfigureAwait(false);

            await WaitForTaskAndCallbackAsync(_batchTaskWithFunctionDto.Task)
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

                bool isFuncPending = _batchTaskWithFunctionDto.Function != null;
                if (isFuncPending)
                {
                    HandleCancellation();
                    return true;
                }

                bool isTaskPending = _batchTaskWithFunctionDto.Task != null
                    && !TaskUtil.IsTaskRunningOrCompleted(_batchTaskWithFunctionDto.Task);

                if (!isTaskPending)
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

            var awaitTask = false;

            if (_batchTaskWithFunctionDto.Function != null)
            {
                _batchTaskWithFunctionDto.Task = _batchTaskWithFunctionDto.Function!.Invoke();
                _batchTaskWithFunctionDto.Function = null;
                _batchTaskWithFunctionDto.FunctionTask?.TrySetResult();
                awaitTask = true;
            }

            await StartTaskAndCallBackAndWaitAsync(_batchTaskWithFunctionDto.Task, awaitTask)
                .ConfigureAwait(false);
        }

        private protected override bool IsTaskCompleted() => 
            _batchTaskWithFunctionDto.Task?.IsCompleted ?? false;

        private void HandleCancellation()
        {
            _batchTaskWithFunctionDto.Task = null;
            _batchTaskWithFunctionDto.Function = null;
        }
    }
}
