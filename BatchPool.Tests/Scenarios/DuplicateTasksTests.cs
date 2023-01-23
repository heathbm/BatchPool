using BatchPool.UnitTests.BasicTests.Helpers;
using BatchPool.UnitTests.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class DuplicateTasksTests
    {
        [Fact]
        public static async Task DuplicateTasks_SameTaskIsNotRunTwice()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();

            var task = new Task(() => progressTracker.IncrementProgress());
            batchTasks.Add(batchPool.Add(task));

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(new Task(() => progressTracker.IncrementProgress())));
            }

            batchTasks.Add(batchPool.Add(task));

            // 100 tests + 2 duplicates have been added
            Assert.Equal(numberOfTasks + 2, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            // 100 tests + 1 distinct has completed, the duplicated tasks did not execute twice
            SharedTests.PostChecks(numberOfTasks + 1, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task DuplicateAsyncFunctions_SameTaskIsNotRunTwice()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();

            var task = new Task(async () => await progressTracker.IncrementProgressAsync());
            batchTasks.Add(batchPool.Add(task));

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(new Task(() => progressTracker.IncrementProgress())));
            }

            batchTasks.Add(batchPool.Add(task));

            // 100 tests + 2 duplicates have been added
            Assert.Equal(numberOfTasks + 2, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            // 100 tests + 1 distinct has completed, the duplicated tasks did not execute twice
            SharedTests.PostChecks(numberOfTasks + 1, progressTracker, batchTasks, batchPool);
        }


        [Fact]
        public static async Task DuplicateActions_SameActionIsRunTwice()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();

            Action task = () => progressTracker.IncrementProgress();
            batchTasks.Add(batchPool.Add(task));

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(() => progressTracker.IncrementProgress()));
            }

            batchTasks.Add(batchPool.Add(task));

            // 100 tests + 2 duplicates have been added
            Assert.Equal(numberOfTasks + 2, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            // 100 tests + 2 distinct has completed, the duplicated actions are treated separately because an Action is essentially a ref to a function and has no state
            SharedTests.PostChecks(numberOfTasks + 2, progressTracker, batchTasks, batchPool);
        }
    }
}
