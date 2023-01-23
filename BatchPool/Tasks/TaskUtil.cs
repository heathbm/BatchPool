namespace BatchPool
{
    internal static class TaskUtil
    {
        internal static bool IsTaskRunningOrCompleted(Task task) =>
            IsTaskRunning(task) ||
            IsCompleted(task);

        private static bool IsTaskRunning(Task task) =>
            task.Status is 
                TaskStatus.Running 
                or TaskStatus.WaitingForChildrenToComplete 
                or TaskStatus.WaitingForActivation 
                or TaskStatus.WaitingToRun;

        private static bool IsCompleted(Task task) =>
            task.IsCompleted;

        internal static async Task<bool> AwaitWithTimeout(this Task task, uint? timeoutInMilliseconds, CancellationToken? cancellationToken = default)
        {
            TimeSpan? timeSpan = 
                timeoutInMilliseconds.HasValue 
                ? TimeSpan.FromMilliseconds((double)timeoutInMilliseconds) 
                : null;

            return await AwaitWithTimeout(task, timeSpan, cancellationToken)
                .ConfigureAwait(false);
        }

        internal static async Task<bool> AwaitWithTimeout(this Task task, TimeSpan? timeout, CancellationToken? cancellationToken = default)
        {
            if (timeout.HasValue &&
                !cancellationToken.HasValue)
            {
                var completedTask = await Task
                    .WhenAny(Task.Delay(timeout.Value), task)
                    .ConfigureAwait(false);
                return completedTask == task;
            }
            else if (!timeout.HasValue &&
                cancellationToken.HasValue)
            {
                var completedTask = await Task
                    .WhenAny(Task.Delay(Timeout.Infinite, cancellationToken.Value), task)
                    .ConfigureAwait(false);
                return completedTask == task;
            }
            else if (timeout.HasValue &&
                cancellationToken.HasValue)
            {
                var completedTask = await Task
                    .WhenAny(Task.Delay(timeout.Value, cancellationToken.Value), task)
                    .ConfigureAwait(false);
                return completedTask == task;
            }
            else
            {
                await task.ConfigureAwait(false);
                return true;
            }
        }
    }
}
