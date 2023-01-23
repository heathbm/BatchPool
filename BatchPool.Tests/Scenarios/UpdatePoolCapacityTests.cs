using BatchPool.UnitTests.BasicTests.Helpers;
using BatchPool.UnitTests.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class UpdatePoolCapacityTests
    {
        [Fact]
        public static async Task ReduceCapacityAndWaitForCapacityToUpdate_IsSuccessful()
        {
            int numberOfTasks = 1000;
            var progressTracker = new ProgressTracker();
            int initBatchSize = 10;
            int newBatchSize = 5;
            var batchPool = new BatchPoolContainer(initBatchSize);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(new Task(() => progressTracker.IncrementProgress())));
                if (numberOfTasks / 2 == taskIndex)
                {
                    await batchPool.UpdateCapacityAsync(newBatchSize);
                }
            }

            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task ReduceCapacityAndForget_IsSuccessful()
        {
            int numberOfTasks = 1000;
            var progressTracker = new ProgressTracker();
            int initBatchSize = 10;
            int newBatchSize = 5;
            var batchPool = new BatchPoolContainer(initBatchSize);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(new Task(() => progressTracker.IncrementProgress())));
                if (numberOfTasks / 2 == taskIndex)
                {
                    batchPool.UpdateCapacityAndForget(newBatchSize);
                }
            }

            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task IncreaseCapacityAndWaitForCapacityToUpdate_IsSuccessful()
        {
            int numberOfTasks = 1000;
            var progressTracker = new ProgressTracker();
            int initBatchSize = 5;
            int newBatchSize = 10;
            var batchPool = new BatchPoolContainer(initBatchSize);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(new Task(() => progressTracker.IncrementProgress())));
                if (numberOfTasks / 2 == taskIndex)
                {
                    await batchPool.UpdateCapacityAsync(newBatchSize);
                }
            }

            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task IncreaseCapacityAndForget_IsSuccessful()
        {
            int numberOfTasks = 1000;
            var progressTracker = new ProgressTracker();
            int initBatchSize = 5;
            int newBatchSize = 10;
            var batchPool = new BatchPoolContainer(initBatchSize);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add(batchPool.Add(new Task(() => progressTracker.IncrementProgress())));
                if (numberOfTasks / 2 == taskIndex)
                {
                    batchPool.UpdateCapacityAndForget(newBatchSize);
                }
            }

            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);
        }
    }
}
