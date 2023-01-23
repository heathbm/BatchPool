using BatchPool.UnitTests.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.DynamicContainerTests
{
    public static class UpdatableIntDynamicContainerTests
    {
        [Fact]
        public static async Task BasicSequentialOrderAsc()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolUpdatableDynamicContainer<int>(batchSize, isEnabled: false);

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
        public static async Task BBasicSequentialOrderAsc_UpdateFirstToLast()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolUpdatableDynamicContainer<int>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, 0);

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, 1);

            var task2 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask2 = batchPool.Add(task2, 2);

            Assert.True(batchPool.UpdatePriority(batchTask0, 3));

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("2", executionOrderTracker.GetNext());
            Assert.Equal("0", executionOrderTracker.GetNext());
        }

        [Fact]
        public static async Task BasicSequentialOrderAscFlipped()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolUpdatableDynamicContainer<int>(batchSize, new IntReverseComparer(), isEnabled: false);

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
        public static async Task BasicSequentialOrderAscFlipped_UpdateFirstToLast()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolUpdatableDynamicContainer<int>(batchSize, new IntReverseComparer(), isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, 0);

            var task1 = new Task(() => executionOrderTracker.Add("1"));
            var batchTask1 = batchPool.Add(task1, 1);

            var task2 = new Task(() => executionOrderTracker.Add("2"));
            var batchTask2 = batchPool.Add(task2, 2);

            Assert.True(batchPool.UpdatePriority(batchTask2, -1));

            Assert.Equal(3, batchPool.GetPendingTaskCount());
            await batchPool.ResumeAndWaitForAllAsync();
            Assert.Equal(0, batchPool.GetPendingTaskCount());

            Assert.Equal("1", executionOrderTracker.GetNext());
            Assert.Equal("0", executionOrderTracker.GetNext());
            Assert.Equal("2", executionOrderTracker.GetNext());
        }

        [Fact]
        public static void UpdateWithSamePriority_ReturnsFalse()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolUpdatableDynamicContainer<int>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, 0);

            Assert.False(batchPool.UpdatePriority(batchTask0, 0));
        }

        [Fact]
        public static void UpdateWithSameBatchPoolTaskThatIsNotAdded_ReturnsFalse()
        {
            var executionOrderTracker = new ExecutionOrderTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolUpdatableDynamicContainer<int>(batchSize, isEnabled: false);

            var task0 = new Task(() => executionOrderTracker.Add("0"));
            var batchTask0 = batchPool.Add(task0, 0);

            var newBatchPoolTask = new BatchPoolContainer(1, isEnabled: false).Add(task0);

            Assert.False(batchPool.UpdatePriority(newBatchPoolTask, 0));
        }
    }
}
