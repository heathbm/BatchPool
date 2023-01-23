using BatchPool.UnitTests.BasicTests.Helpers;
using BatchPool.UnitTests.BasicTests.Helpers.ClassData;
using BatchPool.UnitTests.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static BatchPool.UnitTests.Utilities.TestConstants;

namespace BatchPool.UnitTests.BasicTests
{
    public static class AddTestsInBatch
    {
        [Theory]
        [ClassData(typeof(AddTasksInBatchData))]
        public static async Task CreateBatchPool_AddTasksInBatchThenWaitForAll_IsSuccessful(
            int numberOfTasks,
            int batchSize,
            bool isEnabled,
            bool runAndForget,
            TaskType taskType)
        {
            for (int testIndex = 0; testIndex < NumberOfTests; testIndex++)
            {
                await CreateBatchPool_AddTasksInBatch(numberOfTasks, batchSize, isEnabled, runAndForget, taskType);
            }
        }

        private static async Task CreateBatchPool_AddTasksInBatch(int numberOfTasks, int batchSize, bool isEnabled, bool runAndForget, TaskType taskType)
        {
            var progressTracker = new ProgressTracker();
            var batchPool = new BatchPoolContainer(batchSize, isEnabled);

            ICollection<BatchPoolTask> batchTasks = await CreateAndAddTasks(numberOfTasks, isEnabled, runAndForget, taskType, progressTracker, batchPool);
            SharedTests.PreChecks(numberOfTasks, isEnabled, runAndForget, batchTasks, progressTracker, batchPool);
            await TestUtility.StartTasksIfRequiredAndWaitForAllTasksToComplete(isEnabled, runAndForget, batchPool);
            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool);
        }

        private static async Task<ICollection<BatchPoolTask>> CreateAndAddTasks(int numberOfTasks, bool isEnabled, bool runAndForget, TaskType taskType, ProgressTracker progressTracker, BatchPoolContainer batchPoolContainer)
        {
            var tasksToAdd = new List<Task>();
            var functionsToAdd = new List<Func<Task>>();
            var actionsToAdd = new List<Action>();

            // Add to a list so they can all be added in a batch
            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                switch (taskType)
                {
                    case TaskType.Task:
                        tasksToAdd.Add(TestUtility.GetTask(progressTracker));
                        break;
                    case TaskType.WrappedTask:
                        tasksToAdd.Add(TestUtility.GetWrappedTask(progressTracker));
                        break;
                    case TaskType.AsyncFunc:
                        functionsToAdd.Add(TestUtility.GetAsyncFunc(progressTracker));
                        break;
                    case TaskType.Action:
                        actionsToAdd.Add(TestUtility.GetAction(progressTracker));
                        break;
                }
            }

            // Add in a batch
            var batchTasks = taskType switch
            {
                TaskType.Task => batchPoolContainer.Add(tasksToAdd),
                TaskType.WrappedTask => batchPoolContainer.Add(tasksToAdd),
                TaskType.AsyncFunc => batchPoolContainer.Add(functionsToAdd),
                TaskType.Action => batchPoolContainer.Add(actionsToAdd),
                _ => throw new NotImplementedException()
            };

            if (!runAndForget && isEnabled)
            {
                await batchPoolContainer.WaitForAllAsync();
            }

            return batchTasks;
        }
    }
}
