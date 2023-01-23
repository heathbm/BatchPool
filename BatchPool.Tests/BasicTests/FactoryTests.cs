using BatchPool.UnitTests.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.BasicTests
{
    public static class FactoryTests
    {
        [Fact]
        public static async Task FromFactory_BasicSequentialOrderAsc()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = BatchPoolFactory.GetDynamicallyOrderedBatchPool<int>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            batchPool.Add(task0, 0);

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            batchPool.Add(task1, 1);

            var task2 = new Task(() => executionOrderTracker.Add("2"));
            batchPool.Add(task2, 2);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("0", executionOrderTracker.GetNext());
            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("2", executionOrderTracker.GetNext());
        }
    }
}
