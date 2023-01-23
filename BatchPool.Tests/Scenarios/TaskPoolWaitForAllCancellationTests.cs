using BatchPool.UnitTests.Utilities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class TaskPoolWaitForAllCancellationTests
    {
        [Fact]
        public static async Task WaitForAllTasksToComplete_WithAlreadyCanceledToken_DoesNotWaitForTask()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true);
            var cancellationTokenSource = new CancellationTokenSource();

            var task = async () =>
            {
                await Task.Delay(500);
                progressTracker.IncrementProgress();
            };
            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());

            // Wait less than task so it won't complete
            cancellationTokenSource.Cancel();
            var result = await batchPool.WaitForAllAsync(cancellationTokenSource.Token);
            Assert.False(result);

            Assert.Equal(0, progressTracker.Progress);
            Assert.False(batchTask.IsCompleted);
        }

        [Fact]
        public static async Task WaitForAllTasksToComplete_CancelTokenImmediately_DoesNotWaitForTask()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true);
            var cancellationTokenSource = new CancellationTokenSource();

            var task = async () =>
            {
                await Task.Delay(500);
                progressTracker.IncrementProgress();
            };
            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());

            // Wait less than task so it won't complete
            var taskResult = batchPool.WaitForAllAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            var result = await taskResult;
            Assert.False(result);

            Assert.Equal(0, progressTracker.Progress);
            Assert.False(batchTask.IsCompleted);
        }

        [Fact]
        public static async Task WaitForAllTasksToComplete_WithAlreadyCanceledTokenAndAlreadyCompletedTask_ReturnsFalse()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true);
            var cancellationTokenSource = new CancellationTokenSource();

            var task = () =>
            {
                progressTracker.IncrementProgress();
            };
            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());
            await Task.Delay(500);
            Assert.Equal(1, progressTracker.Progress);
            Assert.True(batchTask.IsCompleted);

            // Wait less than task so it won't complete
            cancellationTokenSource.Cancel();
            var result = await batchPool.WaitForAllAsync(cancellationTokenSource.Token);
            Assert.False(result);
        }

        [Fact]
        public static async Task WaitForAllTasksToComplete_CancelTokenImmediatelyWithAlreadyCompletedTask_ReturnsTrue()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true);
            var cancellationTokenSource = new CancellationTokenSource();

            var task = () =>
            {
                progressTracker.IncrementProgress();
            };
            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());
            await Task.Delay(500);
            Assert.Equal(1, progressTracker.Progress);
            Assert.True(batchTask.IsCompleted);

            // Wait less than task so it won't complete
            var taskResult = batchPool.WaitForAllAsync(cancellationTokenSource.Token);
            cancellationTokenSource.Cancel();
            var result = await taskResult;

            // Already complete so will be true
            Assert.True(result);
        }

        [Fact]
        public static async Task CancelTokenOfBatchPool_AddANewTask_ThrowsException()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var cancellationTokenSource = new CancellationTokenSource();
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true, cancellationToken: cancellationTokenSource.Token);

            var task = () =>
            {
                progressTracker.IncrementProgress();
            };

            var batchTask = batchPool.Add(task);

            Assert.Equal(0, batchPool.GetPendingTaskCount());
            await Task.Delay(500);
            Assert.Equal(1, progressTracker.Progress);
            Assert.True(batchTask.IsCompleted);

            // Wait less than task so it won't complete
            cancellationTokenSource.Cancel();
            Assert.Throws<OperationCanceledException>(() => batchPool.Add(task));
        }
    }
}
