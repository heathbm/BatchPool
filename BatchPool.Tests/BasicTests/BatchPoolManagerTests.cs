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

            var batchPoolManager = new BatchPoolContainerManager<BatchPoolContainer>();

            var batchPool = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetQueueBatchPool(batchSize));

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

            var batchPoolManager = new BatchPoolContainerManager<BatchPoolContainer>();

            batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetQueueBatchPool(batchSize));
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

            var batchPoolManager = new BatchPoolContainerManager<BatchPoolContainer>();

            var batchPool = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetQueueBatchPool(batchSize));

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

            var batchPoolManager = new BatchPoolContainerManager<BatchPoolContainer>();

            var batchPool1 = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetQueueBatchPool(batchSize));
            var batchPool2 = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetQueueBatchPool(batchSize));
                
            Assert.Equal(batchPool1, batchPool2);
        }

        [Fact]
        public static void TryToCreateBatchPoolWithSameName_ReturnsSameBatchPoolViaRetrieval()
        {
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager<BatchPoolContainer>();

            var batchPool1 = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetQueueBatchPool(batchSize));
            var batchPool2 = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetQueueBatchPool(batchSize));
            var isBatchPoolFound = batchPoolManager.TryGetBatchPool("123", out BatchPoolContainer? batchPoolR);
            Assert.True(isBatchPoolFound);

            Assert.Equal(batchPool1, batchPoolR);
            Assert.Equal(batchPool2, batchPoolR);
        }

        [Fact]
        public static void TryToCreateAndRegisterDynamicBatchPool_ReturnsCorrectType()
        {
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager<BatchPoolDynamicContainer<string>>();

            var batchPool1 = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetDynamicallyOrderedBatchPool<string>(batchSize));

            Assert.Equal(typeof(BatchPoolDynamicContainer<string>), batchPool1.GetType());
        }

        [Fact]
        public static void TryToCreateAndRegisterUpdatableDynamicBatchPool_ReturnsCorrectType()
        {
            int batchSize = 5;

            var batchPoolManager = new BatchPoolContainerManager<BatchPoolUpdatableDynamicContainer<string>>();

            var batchPool1 = batchPoolManager.RegisterBatchPool("123", BatchPoolFactory.GetUpdatableDynamicallyOrderedBatchPool<string>(batchSize));

            Assert.Equal(typeof(BatchPoolUpdatableDynamicContainer<string>), batchPool1.GetType());
        }
    }
}
