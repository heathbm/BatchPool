using BatchPool.UnitTests.BasicTests.Helpers;
using BatchPool.UnitTests.Utilities;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class TaskExecutionOrderTests
    {
        [Fact]
        public static async Task TaskExecutionOrderIsRespected_WithBatchSizeGreaterThan1()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();
            var concurrentQueue = new ConcurrentQueue<int>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() =>
                {
                    lock (progressTracker)
                    {
                        concurrentQueue.Enqueue(progressTracker.IncrementProgress());
                    }
                }));
                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);

            var lastItem = 0;
            foreach (var currentItem in concurrentQueue)
            {
                Assert.Equal(lastItem + 1, currentItem);
                lastItem = currentItem;
            }
        }

        [Fact]
        public static async Task TaskExecutionOrderIsRespected_WithBatchSizeSetTo1()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();
            var concurrentQueue = new ConcurrentQueue<int>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() =>
                {
                    concurrentQueue.Enqueue(progressTracker.IncrementProgress());
                }));
                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);

            var lastItem = 0;
            foreach (var currentItem in concurrentQueue)
            {
                Assert.Equal(lastItem + 1, currentItem);
                lastItem = currentItem;
            }
        }
    }
}
