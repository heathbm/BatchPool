using BatchPool.UnitTests.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.DynamicContainerTests
{
    public static class IntDynamicContainerTests
    {
        [Fact]
        public static async Task BasicSequentialOrderAsc()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<int>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, 0);

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, 1);

            var task2 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask2 = batchPool.Add(task2, 2);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("0", executionOrderTracker.GetNext());
            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("2", executionOrderTracker.GetNext());
        }

        [Fact]
        public static async Task BasicSequentialOrderAscAddedInDesc()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<int>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask0 = batchPool.Add(task0, 2);

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, 1);

            var task2 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask2 = batchPool.Add(task2, 0);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("0", executionOrderTracker.GetNext());
            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("2", executionOrderTracker.GetNext());
        }

        [Fact]
        public static async Task BasicSequentialOrderAscFlipped()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<int>(batchSize, new IntReverseComparer(), isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, 0);

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, 1);

            var task2 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask2 = batchPool.Add(task2, 2);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("2", executionOrderTracker.GetNext());
            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("0", executionOrderTracker.GetNext());
        }

        [Fact]
        public static async Task BasicSequentialOrderAscAddedInDescFlipped()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<int>(batchSize, new IntReverseComparer(), isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask0 = batchPool.Add(task0, 2);

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, 1);

            var task2 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask2 = batchPool.Add(task2, 0);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("2", executionOrderTracker.GetNext());
            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("0", executionOrderTracker.GetNext());
        }
    }

    internal class IntReverseComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return y.CompareTo(x);
        }
    }
}
