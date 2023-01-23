using BatchPool.UnitTests.BasicTests.Helpers;
using BatchPool.UnitTests.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class WaitForAllTests
    {
        [Fact]
        public static async Task CreateBatchPool_AddTasksInBatchThenWaitForAllViaStaticMethod_IsSuccessful()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(async () => await progressTracker.IncrementProgressAsync()));
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await BatchPoolContainer.WaitForAllAsync(batchTasks);

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task CreateBatchPool_AddTasksInBatchThenWaitForAllViaExtensionMethod_IsSuccessful()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(async () => await progressTracker.IncrementProgressAsync()));
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchTasks.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);
        }
    }
}
