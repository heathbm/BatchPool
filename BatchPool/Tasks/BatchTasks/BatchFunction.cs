namespace BatchPool
{
    /// <inheritdoc/>
    internal class BatchFunction : BatchPoolTask
    {
        FunctionContainer _functionContainer;

        internal BatchFunction(Func<Task> function, ICallback? callback)
            : base(callback)
        {
            _functionContainer = new(function);
        }

        /// <inheritdoc/>
        public override bool IsCompleted => 
            !IsCancelled 
            && IsTaskCompleted();

        /// <inheritdoc/>
        public override bool IsCancelled => 
            _functionContainer.Task == null 
            && _functionContainer.Function == null;

        /// <inheritdoc/>
        public override async Task WaitForTaskAsync()
        {
            if (IsCancelled)
            {
                return;
            }

            await _functionContainer
                .WaitForTaskToComplete()
                .ConfigureAwait(false);

            await WaitForTaskAndCallbackAsync(_functionContainer.Task)
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

                bool isFuncPending = _functionContainer.Function != null;
                if (isFuncPending)
                {
                    HandleCancellation();
                    return true;
                }

                bool isTaskPending = _functionContainer.Task != null
                    && !TaskUtil.IsTaskRunningOrCompleted(_functionContainer.Task);

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

            if (_functionContainer.Function != null)
            {
                _functionContainer.Task = _functionContainer.Function!.Invoke();
                _functionContainer.Function = null;
                _functionContainer.FunctionTask?.TrySetResult();
                awaitTask = true;
            }

            await StartTaskAndCallBackAndWaitAsync(_functionContainer.Task, awaitTask)
                .ConfigureAwait(false);
        }

        private protected override bool IsTaskCompleted() => 
            _functionContainer.Task?.IsCompleted ?? false;

        private void HandleCancellation()
        {
            _functionContainer.Task = null;
            _functionContainer.Function = null;
        }
    }
}
