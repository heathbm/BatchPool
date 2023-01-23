namespace BatchPool
{
    internal struct ActionCallback : ICallback
    {
        private Task? _taskCallback;
        private Action? _actionCallback;
        /// <summary>
        /// This task will be completed once the _actionCallback /> has been run.
        /// </summary>
        private readonly TaskCompletionSource? _callbackTask;
        private readonly bool _waitForCallback;

        internal ActionCallback(Action actionCallback, bool waitForCallback)
        {
            _taskCallback = null;
            _actionCallback = actionCallback;
            _waitForCallback = waitForCallback;
            _callbackTask = waitForCallback ? new() : null;
        }

        public async Task WaitForCallbackIfRequired()
        {
            if (_waitForCallback
                && _callbackTask != null
                && !_callbackTask.Task.IsCompleted)
            {
                await _callbackTask.Task.ConfigureAwait(false);
            }
        }

        public async Task RunCallbackIfPresent()
        {
            if (_actionCallback != null)
            {
                RunCallback();
            }

            if (_waitForCallback
                && _taskCallback != null)
            {
                await _taskCallback.ConfigureAwait(false);
            }

            _callbackTask?.TrySetResult();
        }

        private void RunCallback()
        {
            lock (_actionCallback!)
            {
                _taskCallback = new Task(_actionCallback);
                _taskCallback.Start();
                _actionCallback = null;
            }
        }
    }
}
