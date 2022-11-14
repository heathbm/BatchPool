using BatchPool.UnitTests.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class PauseAndResumeTests
    {
        [Fact]
        public static async Task PauseAndResumeBetweenTasks_IsSuccessful()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var shortDelay = TimeSpan.FromMilliseconds(10);
            // Disable the BatchPoolContainer so we can verify both tasks have been added before it starts
            bool isEnabled = false;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled);

            var firstTask = batchPool.Add(async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            });

            var secondTask = batchPool.Add(async () =>
            {
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            });

            // Wait longer that the task delay to assess
            await Task.Delay(500);

            // Ensure no tasks completed
            Assert.Equal(0, progressTracker.Progress);
            // Ensure both tasks are waiting
            Assert.Equal(2, batchPool.GetPendingTaskCount());

            // Start the BatchPoolContainer
            batchPool.ResumeAndForget();
            // Immediately pause it to prevent the second task from starting
            batchPool.Pause();

            // Wait for first batch to finish
            await firstTask.WaitForTaskAsync();

            // Ensure only first task completed
            Assert.True(firstTask.IsCompleted);
            Assert.Equal(1, progressTracker.Progress);
            // Ensure only 1 task is waiting
            Assert.Equal(1, batchPool.GetPendingTaskCount());

            // Resume to start second task
            batchPool.ResumeAndForget();
            await secondTask.WaitForTaskAsync();

            // Ensure all tasks are complete and no tasks are waiting
            Assert.True(secondTask.IsCompleted);
            Assert.Equal(2, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }

        [Fact]
        public static async Task PauseBeforeTasks_IsSuccessful()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var shortDelay = TimeSpan.FromMilliseconds(10);
            // Disable the BatchPoolContainer so we can verify both tasks have been added before it starts
            bool isEnabled = true;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled);
            batchPool.Pause();

            var firstTask = batchPool.Add(async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            });

            var secondTask = batchPool.Add(async () =>
            {
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            });

            // Wait longer that the task delay to assess
            await Task.Delay(500);

            // Ensure no tasks completed
            Assert.Equal(0, progressTracker.Progress);
            // Ensure both tasks are waiting
            Assert.Equal(2, batchPool.GetPendingTaskCount());

            // Start the BatchPoolContainer
            await batchPool.ResumeAndWaitForAllAsync();

            // Ensure all tasks are complete and no tasks are waiting
            Assert.True(firstTask.IsCompleted);
            Assert.True(secondTask.IsCompleted);
            Assert.Equal(2, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }

        [Fact]
        public static async Task PauseAfterTasks_IsSuccessful()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var shortDelay = TimeSpan.FromMilliseconds(10);
            // Disable the BatchPoolContainer so we can verify both tasks have been added before it starts
            bool isEnabled = true;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled);

            var firstTask = batchPool.Add(async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            });

            var secondTask = batchPool.Add(async () =>
            {
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            });

            batchPool.Pause();

            // Wait longer that the task delay to assess
            await Task.Delay(500);

            // Ensure only 1 task started
            Assert.Equal(1, progressTracker.Progress);
            // Ensure only 1 task is waiting
            Assert.Equal(1, batchPool.GetPendingTaskCount());

            // Start the BatchPoolContainer
            await batchPool.ResumeAndWaitForAllAsync();

            // Ensure all tasks are complete and no tasks are waiting
            Assert.True(firstTask.IsCompleted);
            Assert.True(secondTask.IsCompleted);
            Assert.Equal(2, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }
    }
}