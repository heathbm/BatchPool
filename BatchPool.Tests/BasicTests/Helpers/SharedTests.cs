using BatchPool.UnitTests.Utilities;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static BatchPool.UnitTests.Utilities.TestConstants;

namespace BatchPool.UnitTests.BasicTests.Helpers
{
    internal static class SharedTests
    {
        internal static void PostChecks(int numberOfTasks, ProgressTracker progressTracker, ICollection<BatchPoolTask> batchTasks, BatchPoolContainer batchPoolContainer, CallBackType? callBackType = null, bool? waitForCallBack = null)
        {
            AssertAllTasksAreComplete(numberOfTasks, progressTracker, batchPoolContainer, batchTasks, callBackType, waitForCallBack);
        }

        internal static void PreChecks(int numberOfTasks, bool isEnabled, bool runAndForget, ICollection<BatchPoolTask> batchTasks, ProgressTracker progressTracker, BatchPoolContainer batchPoolContainer, CallBackType? callBackType = null, bool? waitForCallBack = null)
        {
            // Ensure all tasks have be added
            Assert.Equal(numberOfTasks, batchTasks.Count);

            if (!isEnabled)
            {
                // Ensure no tests have completed
                Assert.Equal(0, progressTracker.Progress);
            }

            if (isEnabled && !runAndForget)
            {
                AssertAllTasksAreComplete(numberOfTasks, progressTracker, batchPoolContainer, batchTasks, callBackType, waitForCallBack);
            }
        }

        private static void AssertAllTasksAreComplete(int numberOfTasks, ProgressTracker progressTracker, BatchPoolContainer batchPoolContainer, ICollection<BatchPoolTask> batchTasks, CallBackType? callBackType = null, bool? waitForCallBack = null)
        {
            Assert.Equal(0, batchPoolContainer.GetPendingTaskCount());

            if (callBackType.HasValue
                && waitForCallBack.HasValue
                && callBackType.Value != CallBackType.None
                && waitForCallBack.Value)
            {
                // Ensure all tests have completed, including callbacks, so double the number of tasks
                Assert.Equal(numberOfTasks * 2, progressTracker.Progress);
            }
            else
            {
                // Ensure all tests have completed
                Assert.Equal(numberOfTasks, progressTracker.Progress);
            }

            Assert.True(batchTasks.All(batchTask => batchTask.IsCompleted));
        }
    }
}
