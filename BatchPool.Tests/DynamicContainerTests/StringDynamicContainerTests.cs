using BatchPool.UnitTests.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.DynamicContainerTests
{
    public static class StringDynamicContainerTests
    {
        [Fact]
        public static async Task BasicSequentialOrderAsc()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<string>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, "a");

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, "b");

            var task2 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask2 = batchPool.Add(task2, "c");

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
            var batchPool = new BatchPoolDynamicContainer<string>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask0 = batchPool.Add(task0, "c");

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, "b");

            var task2 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask2 = batchPool.Add(task2, "a");

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
            var batchPool = new BatchPoolDynamicContainer<string>(batchSize, new StringReverseComparer(), isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, "a");

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, "b");

            var task2 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask2 = batchPool.Add(task2, "c");

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
            var batchPool = new BatchPoolDynamicContainer<string>(batchSize, new StringReverseComparer(), isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask0 = batchPool.Add(task0, "c");

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, "b");

            var task2 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask2 = batchPool.Add(task2, "a");

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("2", executionOrderTracker.GetNext());
            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("0", executionOrderTracker.GetNext());
        }
    }

    internal class StringReverseComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return y!.CompareTo(x!);
        }
    }
}
