namespace BatchPool
{
    internal readonly struct TaskCallback : ICallback
    {
        private readonly Task _taskCallback;
        private readonly bool _waitForCallback;

        internal TaskCallback(Task taskCallback, bool waitForCallback)
        {
            _taskCallback = taskCallback;
            _waitForCallback = waitForCallback;
        }

        public async Task RunCallbackIfPresent()
        {
            var awaitCallbackTask = false;

            if (!TaskUtil.IsTaskRunningOrCompleted(_taskCallback))
            {
                _taskCallback.Start();
                awaitCallbackTask = true;
            }

            if (awaitCallbackTask
                && _waitForCallback)
            {
                await _taskCallback.ConfigureAwait(false);
            }
        }

        public async Task WaitForCallbackIfRequired()
        {
            if (_waitForCallback)
            {
                await _taskCallback.ConfigureAwait(false);
            }
        }
    }
}
