using BatchPool.UnitTests.BasicTests.Helpers;
using BatchPool.UnitTests.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BatchPool.Tasks.BatchTasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class CancellationTests
    {
        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelOneBeforeStartingProcessing_AllProcessExceptCancelledOne()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            BatchPoolTask? middleTask = null;
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() => progressTracker.IncrementProgress()));

                if ((numberOfTasks / 2) == taskIndex)
                {
                    middleTask = newBatchTask;
                    continue;
                }

                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            var isCancelled = middleTask!.Cancel();
            Assert.True(isCancelled);
            Assert.Equal(numberOfTasks - 1, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks - 1, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelMultipleBeforeStartingProcessing_AllProcessExceptCancelledOnes()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var middleTasks = new List<BatchPoolTask>();
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() => progressTracker.IncrementProgress()));

                if ((numberOfTasks / 2) == taskIndex)
                {
                    middleTasks.Add(newBatchTask);
                    continue;
                }

                if ((numberOfTasks / 3) == taskIndex)
                {
                    middleTasks.Add(newBatchTask);
                    continue;
                }

                if ((numberOfTasks / 4) == taskIndex)
                {
                    middleTasks.Add(newBatchTask);
                    continue;
                }

                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            foreach (var task in middleTasks)
            {
                var isCancelled = task.Cancel();
                Assert.True(isCancelled);
            }

            Assert.Equal(numberOfTasks - 3, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks - 3, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelAndRemoveOneBeforeStartingProcessing_AllProcessExceptCancelledAndRemovedOne()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            BatchPoolTask? middleTask = null;
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() => progressTracker.IncrementProgress()));

                if ((numberOfTasks / 2) == taskIndex)
                {
                    middleTask = newBatchTask;
                    continue;
                }

                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            var isCancelled = BatchPoolContainer.RemoveAndCancel(middleTask!);
            Assert.True(isCancelled);

            Assert.Equal(numberOfTasks - 1, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks - 1, progressTracker, batchTasks, batchPool);
        }


        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelAndRemoveMultipleBeforeStartingProcessing_AllProcessExceptCancelledAndRemovedOnes()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var middleTasks = new List<BatchPoolTask>();
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() => progressTracker.IncrementProgress()));

                if ((numberOfTasks / 2) == taskIndex)
                {
                    middleTasks.Add(newBatchTask);
                    continue;
                }

                if ((numberOfTasks / 3) == taskIndex)
                {
                    middleTasks.Add(newBatchTask);
                    continue;
                }

                if ((numberOfTasks / 4) == taskIndex)
                {
                    middleTasks.Add(newBatchTask);
                    continue;
                }

                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            BatchPoolContainer.RemoveAndCancel(middleTasks);

            Assert.Equal(numberOfTasks - 3, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            SharedTests.PostChecks(numberOfTasks - 3, progressTracker, batchTasks, batchPool);
        }

        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelOneWhileProcessing_AllProcessExceptCancelledOne()
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

            // Cancel while BatchPoolContainer is active
            secondTask.Cancel();

            // Wait for first batch to finish
            await firstTask.WaitForTaskAsync();

            // Ensure only first task completed
            Assert.True(firstTask.IsCompleted);
            Assert.Equal(1, progressTracker.Progress);

            // Resume to start second task
            batchPool.ResumeAndForget();
            await secondTask.WaitForTaskAsync();

            // Ensure all tasks are complete and no tasks are waiting
            Assert.False(secondTask.IsCompleted);
            Assert.True(secondTask.IsCancelled);
            Assert.Equal(1, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }

        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelOneWithCallbackWhileProcessing_AllProcessExceptCancelledOneAndItsCallback()
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
            }, async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            },
            waitForCallback: true);

            var secondTask = batchPool.Add(async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            }, async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            },
            waitForCallback: true);

            // Wait longer that the task delay to assess
            await Task.Delay(500);

            // Ensure no tasks completed
            Assert.Equal(0, progressTracker.Progress);
            // Ensure both tasks are waiting
            Assert.Equal(2, batchPool.GetPendingTaskCount());

            // Start the BatchPoolContainer
            batchPool.ResumeAndForget();

            // Cancel while BatchPoolContainer is active
            secondTask.Cancel();

            // Wait for first batch to finish
            await firstTask.WaitForTaskAsync();

            // Ensure only first task completed
            Assert.True(firstTask.IsCompleted);
            Assert.Equal(2, progressTracker.Progress);

            // Resume to start second task
            batchPool.ResumeAndForget();
            await secondTask.WaitForTaskAsync();

            // Ensure all tasks are complete and no tasks are waiting
            Assert.False(secondTask.IsCompleted);
            Assert.True(secondTask.IsCancelled);
            Assert.Equal(2, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }

        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelAndRemoveOneWhileProcessing_AllProcessExceptCancelledOne()
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
                // Create a delay so we can pause the BatchPoolContainer before this ends
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

            // Cancel while BatchPoolContainer is active
            BatchPoolContainer.RemoveAndCancel(secondTask);

            // Wait for first batch to finish
            await firstTask.WaitForTaskAsync();

            // Ensure only first task completed
            Assert.True(firstTask.IsCompleted);
            Assert.Equal(1, progressTracker.Progress);

            // Resume to start second task
            batchPool.ResumeAndForget();
            await secondTask.WaitForTaskAsync();

            // Ensure all tasks are complete and no tasks are waiting
            Assert.False(secondTask.IsCompleted);
            Assert.True(secondTask.IsCancelled);
            Assert.Equal(1, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }

        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelAndRemoveOneWithCallbackWhileProcessing_AllProcessExceptCancelledOneAndItsCallback()
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
            }, async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            },
            waitForCallback: true);

            var secondTask = batchPool.Add(async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            }, async () =>
            {
                // Create a delay so we can pause the BatchPoolContainer before this ends
                await Task.Delay(shortDelay);
                progressTracker.IncrementProgress();
            },
            waitForCallback: true);

            // Wait longer that the task delay to assess
            await Task.Delay(500);

            // Ensure no tasks completed
            Assert.Equal(0, progressTracker.Progress);
            // Ensure both tasks are waiting
            Assert.Equal(2, batchPool.GetPendingTaskCount());

            // Start the BatchPoolContainer
            batchPool.ResumeAndForget();

            // Cancel while BatchPoolContainer is active
            BatchPoolContainer.RemoveAndCancel(secondTask);

            // Wait for first batch to finish
            await firstTask.WaitForTaskAsync();

            // Ensure only first task completed
            Assert.True(firstTask.IsCompleted);
            Assert.Equal(2, progressTracker.Progress);

            // Resume to start second task
            batchPool.ResumeAndForget();
            await secondTask.WaitForTaskAsync();

            // Ensure all tasks are complete and no tasks are waiting
            Assert.False(secondTask.IsCompleted);
            Assert.True(secondTask.IsCancelled);
            Assert.Equal(2, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }

        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelAndRemoveAll_NoneProcess()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() => progressTracker.IncrementProgress()));
                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            batchPool.RemoveAndCancelPendingTasks();

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            Assert.True(batchTasks.TrueForAll(batchTask => batchTask.IsCancelled));
            Assert.True(batchTasks.TrueForAll(batchTask => !batchTask.IsCompleted));
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }


        [Fact]
        public static async Task CreateBatchPoolAndAddTasks_CancelAll_NoneProcess()
        {
            int numberOfTasks = 100;
            var progressTracker = new ProgressTracker();
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);
            var batchTasks = new List<BatchPoolTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = batchPool.Add(new Task(() => progressTracker.IncrementProgress()));
                batchTasks.Add(newBatchTask);
            }

            Assert.Equal(numberOfTasks, batchPool.GetPendingTaskCount());

            foreach (var task in batchTasks)
            {
                task.Cancel();
            }

            batchPool.ResumeAndForget();
            await batchPool.WaitForAllAsync();

            Assert.True(batchTasks.TrueForAll(batchTask => batchTask.IsCancelled));
            Assert.True(batchTasks.TrueForAll(batchTask => !batchTask.IsCompleted));
            Assert.Equal(0, batchPool.GetPendingTaskCount());
        }

        [Fact]
        public static async Task CancelTaskBeforeCompletion_WorksAndSemaphoreStillContinues()
        {
            var progressTracker = new ProgressTracker();
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false);

            var firstTask = batchPool.Add(async () =>
            {
                await Task.Delay(500);
                lock (progressTracker)
                {
                    progressTracker.IncrementProgress();
                }
            });

            Assert.Equal(1, batchPool.GetPendingTaskCount());

            batchPool.ResumeAndForget();
            var isCancelled = firstTask.Cancel();
            Assert.False(isCancelled);
            Assert.Equal(0, progressTracker.Progress);
            Assert.Equal(0, batchPool.GetPendingTaskCount());
            Assert.Equal(0, progressTracker.Progress);

            await batchPool.WaitForAllAsync();

            Assert.Equal(0, batchPool.GetPendingTaskCount());
            // It was already running so it can't be cancelled
            Assert.False(firstTask.IsCancelled);
            Assert.True(firstTask.IsCompleted);

            // Ensure carries on

            var secondTask = batchPool.Add(new Task(() =>
            {
                lock (progressTracker)
                {
                    progressTracker.IncrementProgress();
                }
            }));

            await batchPool.WaitForAllAsync();
            await firstTask.WaitForTaskAsync();
            await secondTask.WaitForTaskAsync();
            Assert.Equal(2, progressTracker.Progress);
        }
    }
}
