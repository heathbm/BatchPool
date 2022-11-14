using BatchPool.UnitTests.Utilities;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class TaskPoolWaitForAllTimeoutsTests
    {
        [Fact]
        public static async Task WaitForLongTaskToCompleteWithShortTimeout_TimeoutOccursBeforeTaskCompletes()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true);

            var task = async () =>
            {
                await Task.Delay(500);
                progressTracker.IncrementProgress();
            };
            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());

            // Wait less than task so it won't complete
            var result = await batchPool.WaitForAllAsync(1);
            Assert.False(result);

            Assert.Equal(0, progressTracker.Progress);
            Assert.False(batchTask.IsCompleted);
        }

        [Fact]
        public static async Task WaitForLongTaskToCompleteWithLongTimeout_TimeoutDoesNotOccurBeforeTaskCompletes()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true);

            var task = async () =>
            {
                await Task.Delay(1);
                progressTracker.IncrementProgress();
            };
            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());

            // Wait more than task so it will complete
            var result = await batchPool.WaitForAllAsync(500);
            Assert.True(result);

            Assert.Equal(1, progressTracker.Progress);
            Assert.True(batchTask.IsCompleted);
        }

        [Fact]
        public static async Task WaitForAll_AfterTaskHasAlreadyComplete_ReturnTrue()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true);

            var task = async () =>
            {
                await Task.Delay(1);
                progressTracker.IncrementProgress();
            };
            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());
            await Task.Delay(500);
            Assert.Equal(1, progressTracker.Progress);
            Assert.True(batchTask.IsCompleted);

            // Wait less than task so it won't complete
            var result = await batchPool.WaitForAllAsync(10);

            // Already complete so will be true
            Assert.True(result);
        }
    }
}
