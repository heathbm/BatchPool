namespace BatchPool
{
    internal struct FunctionContainer : ITaskContainer
    {
        /// <summary>
        /// This task will be completed once the _actionCallback has been run.
        /// </summary>
        internal readonly TaskCompletionSource? FunctionTask;
        internal Func<Task>? Function;

        internal FunctionContainer(Func<Task> function)
        {
            Task = null;
            Function = function;
            FunctionTask = new TaskCompletionSource();
        }

        public Task? Task { get; set; }

        internal async Task WaitForTaskToComplete()
        {
            if (FunctionTask != null 
                && !FunctionTask.Task.IsCompleted)
            {
                await FunctionTask.Task.ConfigureAwait(false);

                if (Task != null)
                {
                    await Task.ConfigureAwait(false);
                }
            }
        }
    }
}
