using BatchPool.UnitTests.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class DisabledTaskPoolTests
    {
        [Fact]
        public static async Task DisabledBatchPool_DoesNotProcessTasks()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);

            var task = new Task(() => progressTracker.IncrementProgress());
            var batchTask = batchPool.Add(task);

            Assert.Equal(1, batchPool.GetPendingTaskCount());

            await Task.Delay(500);

            Assert.Equal(1, batchPool.GetPendingTaskCount());
            Assert.Equal(0, progressTracker.Progress);
            Assert.False(batchTask.IsCompleted);
        }
    }
}
