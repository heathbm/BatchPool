namespace BatchPool
{
    internal struct FunctionCallback : ICallback
    {
        private Task? _taskCallback;
        private Func<Task>? _functionCallback;
        private readonly TaskCompletionSource? _callbackTask;
        private readonly bool _waitForCallback;

        internal FunctionCallback(Func<Task> functionCallback, bool waitForCallback)
        {
            _taskCallback = null;
            _functionCallback = functionCallback;
            _waitForCallback = waitForCallback;
            _callbackTask = waitForCallback ? new() : null;
        }

        public async Task WaitForCallbackIfRequired()
        {
            if (_waitForCallback 
                && !_callbackTask!.Task.IsCompleted)
            {
                await _callbackTask.Task.ConfigureAwait(false);
            }
        }

        public async Task RunCallbackIfPresent()
        {
            var awaitCallbackTask = false;

            if (_taskCallback != null)
            {
                if (!TaskUtil.IsTaskRunningOrCompleted(_taskCallback))
                {
                    // Start the task.
                    _taskCallback.Start();
                    awaitCallbackTask = true;
                }
            }
            else if (_functionCallback != null)
            {
                lock (_functionCallback)
                {
                    // Invoke the function to retrieve the task to be awaited.
                    _taskCallback = _functionCallback!.Invoke();
                    _functionCallback = null;
                    awaitCallbackTask = true;
                }
            }

            if (_taskCallback != null
                && awaitCallbackTask
                && _waitForCallback)
            {
                await _taskCallback.ConfigureAwait(false);
            }

            _callbackTask?.TrySetResult();
        }
    }
}
