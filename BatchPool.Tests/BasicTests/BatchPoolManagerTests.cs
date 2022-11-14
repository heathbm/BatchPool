using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.BasicTests
{
    public static class BatchPoolManagerTests
    {
        [Fact]
        public static async Task CreateBatchPoolViaBatchPoolManager_CreateAndRunTasksThenWaitForAll_IsSuccessful()
        {
            int numberOfTasks = 1000;
            int localProgress = 0;
            int taskProgress = 0;
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager();

            var batchPool = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchPool.Add(new Task(() => Interlocked.Increment(ref taskProgress)));
                localProgress++;
            }

            Assert.Equal(numberOfTasks, localProgress);

            await batchPool.WaitForAllAsync();

            Assert.Equal(numberOfTasks, taskProgress);
        }

        [Fact]
        public static async Task CreateBatchPoolViaBatchPoolManager_RetrieveBachPoolThenCreateAndRunTasksThenWaitForAll_IsSuccessful()
        {
            int numberOfTasks = 1000;
            int localProgress = 0;
            int taskProgress = 0;
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager();

            batchPoolManager.CreateAndRegisterBatch("123", batchSize);
            var isBatchPoolFound = batchPoolManager.TryGetBatchPool("123", out BatchPoolContainer? batchPool);
            Assert.True(isBatchPoolFound);

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchPool!.Add(new Task(() => Interlocked.Increment(ref taskProgress)));
                localProgress++;
            }

            Assert.Equal(numberOfTasks, localProgress);

            await batchPool!.WaitForAllAsync();

            Assert.Equal(numberOfTasks, taskProgress);
        }

        [Fact]
        public static async Task CreateBatchPoolViaBatchPoolManager_CreateAndRunTasksThenWaitForAllViaManager_IsSuccessful()
        {
            int numberOfTasks = 1000;
            int localProgress = 0;
            int taskProgress = 0;
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager();

            var batchPool = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchPool.Add(new Task(() => Interlocked.Increment(ref taskProgress)));
                localProgress++;
            }

            Assert.Equal(numberOfTasks, localProgress);

            await batchPoolManager.WaitForAllBatchPools();

            Assert.Equal(numberOfTasks, taskProgress);
        }

        [Fact]
        public static void TryToCreateBatchPoolWithSameName_ReturnsSameBatchPool()
        {
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);
            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            Assert.Equal(batchPool1, batchPool2);
        }

        [Fact]
        public static void TryToCreateBatchPoolWithSameName_ReturnsSameBatchPoolViaRetrieval()
        {
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);
            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);
            var isBatchPoolFound = batchPoolManager.TryGetBatchPool("123", out BatchPoolContainer? batchPoolR);
            Assert.True(isBatchPoolFound);

            Assert.Equal(batchPool1, batchPoolR);
            Assert.Equal(batchPool2, batchPoolR);
        }
    }
}
