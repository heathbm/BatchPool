using BatchPool.UnitTests.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.DynamicContainerTests
{
    public static class EnumDynamicContainerTests
    {
        [Fact]
        public static async Task BasicSequentialOrderAsc()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<TestEnum>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add(TestEnum.High.ToString()));
            var batchTask0 = batchPool.Add(task0, TestEnum.High);

            var task1 = new Task(() => executionOrderTracker.Add(TestEnum.Medium.ToString()));
            var batchTask1 = batchPool.Add(task1, TestEnum.Medium);

            var task2 = new Task(() => executionOrderTracker.Add(TestEnum.Low.ToString()));
            var batchTask2 = batchPool.Add(task2, TestEnum.Low);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal(TestEnum.High.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.Medium.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.Low.ToString(), executionOrderTracker.GetNext());
        }

        [Fact]
        public static async Task BasicSequentialOrderAscAddedInDesc()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<TestEnum>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add(TestEnum.Low.ToString()));
            var batchTask0 = batchPool.Add(task0, TestEnum.Low);

            var task1 = new Task(() => executionOrderTracker.Add(TestEnum.Medium.ToString()));
            var batchTask1 = batchPool.Add(task1, TestEnum.Medium);

            var task2 = new Task(() => executionOrderTracker.Add(TestEnum.High.ToString()));
            var batchTask2 = batchPool.Add(task2, TestEnum.High);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal(TestEnum.High.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.Medium.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.Low.ToString(), executionOrderTracker.GetNext());
        }

        [Fact]
        public static async Task BasicSequentialOrderAscFlipped()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<TestEnum>(batchSize, new EnumReverseComparer(), isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add(TestEnum.High.ToString()));
            var batchTask0 = batchPool.Add(task0, TestEnum.High);

            var task1 = new Task(() => executionOrderTracker.Add(TestEnum.Medium.ToString()));
            var batchTask1 = batchPool.Add(task1, TestEnum.Medium);

            var task2 = new Task(() => executionOrderTracker.Add(TestEnum.Low.ToString()));
            var batchTask2 = batchPool.Add(task2, TestEnum.Low);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal(TestEnum.Low.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.Medium.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.High.ToString(), executionOrderTracker.GetNext());
        }

        [Fact]
        public static async Task BasicSequentialOrderAscAddedInDescFlipped()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolDynamicContainer<TestEnum>(batchSize, new EnumReverseComparer(), isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add(TestEnum.Low.ToString()));
            var batchTask0 = batchPool.Add(task0, TestEnum.Low);

            var task1 = new Task(() => executionOrderTracker.Add(TestEnum.Medium.ToString()));
            var batchTask1 = batchPool.Add(task1, TestEnum.Medium);

            var task2 = new Task(() => executionOrderTracker.Add(TestEnum.High.ToString()));
            var batchTask2 = batchPool.Add(task2, TestEnum.High);

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal(TestEnum.Low.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.Medium.ToString(), executionOrderTracker.GetNext());
            Assert.Equal(TestEnum.High.ToString(), executionOrderTracker.GetNext());
        }
    }

    internal enum TestEnum
    {
        Critical = 0,
        High = 1,
        Medium = 2,
        Low = 3
    }

    internal class EnumReverseComparer : IComparer<TestEnum>
    {
        public int Compare(TestEnum x, TestEnum y)
        {
            return y.CompareTo(x);
        }
    }
}
