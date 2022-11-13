using System;
using System.Threading.Tasks;

namespace BatchPool.UnitTests.Utilities
{
    internal static class TestUtility
    {
        internal static Task GetTask(ProgressTracker progressTracker)
        {
            return new Task(() => progressTracker.IncrementProgress());
        }

        internal static Task<Task> GetWrappedTask(ProgressTracker progressTracker)
        {
            return new Task<Task>(async () => await progressTracker.IncrementProgressAsync());
        }

        internal static Func<Task> GetAsyncFunc(ProgressTracker progressTracker)
        {
            return async () => await progressTracker.IncrementProgressAsync();
        }

        internal static Action GetAction(ProgressTracker progressTracker)
        {
            return () => progressTracker.IncrementProgress();
        }

        internal static async Task StartTasksIfRequiredAndWaitForAllTasksToComplete(bool isEnabled, bool runAndForget, BatchPool batchPool)
        {
            if (!isEnabled)
            {
                if (runAndForget)
                {
                    batchPool.ResumeAndForget();
                }
                else
                {
                    await batchPool.ResumeAndWaitForAllAsync();
                }
            }

            if (runAndForget || !isEnabled)
            {
                await batchPool.WaitForAllAsync();
            }
        }
    }
}
