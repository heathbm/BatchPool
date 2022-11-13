using BatchPool.UnitTests.BasicTests.Helpers;
using BatchPool.UnitTests.BasicTests.Helpers.ClassData;
using BatchPool.UnitTests.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BatchPool.Tasks.BatchTasks;
using Xunit;
using static BatchPool.UnitTests.Utilities.TestConstants;

namespace BatchPool.UnitTests.BasicTests
{
    public class AddTestsIndividually
    {
        [Theory]
        [ClassData(typeof(AddTasksIndividuallyData))]
        public async Task CreateBatchPool_AddTasksIndividuallyThenWaitForAll_IsSuccessful(
            int numberOfTasks,
            int batchSize,
            bool isEnabled,
            bool runAndForget,
            TaskType taskType,
            CallBackType callBackType,
            bool waitForCallBack)
        {
            for (int testIndex = 0; testIndex < NumberOfTests; testIndex++)
            {
                await CreateBatchPool_AddTasksIndividually(numberOfTasks, batchSize, isEnabled, runAndForget, taskType, callBackType, waitForCallBack);
            }
        }

        private static async Task CreateBatchPool_AddTasksIndividually(int numberOfTasks, int batchSize, bool isEnabled, bool runAndForget, TaskType taskType, CallBackType callBackType, bool waitForCallBack)
        {
            var progressTracker = new ProgressTracker();
            var batchPool = new BatchPool(batchSize, isEnabled);

            List<BatchTask> batchTasks = await CreateAndAddTasks(numberOfTasks, isEnabled, runAndForget, taskType, callBackType, waitForCallBack, progressTracker, batchPool);
            SharedTests.PreChecks(numberOfTasks, isEnabled, runAndForget, batchTasks, progressTracker, batchPool, callBackType, waitForCallBack);
            await TestUtility.StartTasksIfRequiredAndWaitForAllTasksToComplete(isEnabled, runAndForget, batchPool);
            SharedTests.PostChecks(numberOfTasks, progressTracker, batchTasks, batchPool, callBackType, waitForCallBack);
        }

        private static async Task<List<BatchTask>> CreateAndAddTasks(int numberOfTasks, bool isEnabled, bool runAndForget, TaskType taskType, CallBackType callBackType, bool waitForCallBack, ProgressTracker progressTracker, BatchPool batchPool)
        {
            var callbackProgressTracker = waitForCallBack ? progressTracker : new ProgressTracker();
            var batchTasks = new List<BatchTask>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                var newBatchTask = taskType switch
                {
                    TaskType.Task =>
                        callBackType switch
                        {
                            CallBackType.None => batchPool.Add(TestUtility.GetTask(progressTracker)),
                            CallBackType.Task => batchPool.Add(TestUtility.GetTask(progressTracker), TestUtility.GetTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.WrappedTask => batchPool.Add(TestUtility.GetTask(progressTracker), TestUtility.GetWrappedTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.AsyncFunc => batchPool.Add(TestUtility.GetTask(progressTracker), TestUtility.GetAsyncFunc(callbackProgressTracker), waitForCallBack),
                            CallBackType.Action => batchPool.Add(TestUtility.GetTask(progressTracker), TestUtility.GetAction(callbackProgressTracker), waitForCallBack),
                            _ => throw new NotImplementedException()
                        },
                    TaskType.WrappedTask =>
                        callBackType switch
                        {
                            CallBackType.None => batchPool.Add(TestUtility.GetWrappedTask(progressTracker)),
                            CallBackType.Task => batchPool.Add(TestUtility.GetWrappedTask(progressTracker), TestUtility.GetTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.WrappedTask => batchPool.Add(TestUtility.GetWrappedTask(progressTracker), TestUtility.GetWrappedTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.AsyncFunc => batchPool.Add(TestUtility.GetWrappedTask(progressTracker), TestUtility.GetAsyncFunc(callbackProgressTracker), waitForCallBack),
                            CallBackType.Action => batchPool.Add(TestUtility.GetWrappedTask(progressTracker), TestUtility.GetAction(callbackProgressTracker), waitForCallBack),
                            _ => throw new NotImplementedException()
                        },
                    TaskType.AsyncFunc =>
                        callBackType switch
                        {
                            CallBackType.None => batchPool.Add(TestUtility.GetAsyncFunc(progressTracker)),
                            CallBackType.Task => batchPool.Add(TestUtility.GetAsyncFunc(progressTracker), TestUtility.GetTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.WrappedTask => batchPool.Add(TestUtility.GetAsyncFunc(progressTracker), TestUtility.GetWrappedTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.AsyncFunc => batchPool.Add(TestUtility.GetAsyncFunc(progressTracker), TestUtility.GetAsyncFunc(callbackProgressTracker), waitForCallBack),
                            CallBackType.Action => batchPool.Add(TestUtility.GetAsyncFunc(progressTracker), TestUtility.GetAction(callbackProgressTracker), waitForCallBack),
                            _ => throw new NotImplementedException()
                        },
                    TaskType.Action =>
                        callBackType switch
                        {
                            CallBackType.None => batchPool.Add(TestUtility.GetAction(progressTracker)),
                            CallBackType.Task => batchPool.Add(TestUtility.GetAction(progressTracker), TestUtility.GetTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.WrappedTask => batchPool.Add(TestUtility.GetAction(progressTracker), TestUtility.GetWrappedTask(callbackProgressTracker), waitForCallBack),
                            CallBackType.AsyncFunc => batchPool.Add(TestUtility.GetAction(progressTracker), TestUtility.GetAsyncFunc(callbackProgressTracker), waitForCallBack),
                            CallBackType.Action => batchPool.Add(TestUtility.GetAction(progressTracker), TestUtility.GetAction(callbackProgressTracker), waitForCallBack),
                            _ => throw new NotImplementedException()
                        },
                    _ => throw new NotImplementedException()
                };

                batchTasks.Add(newBatchTask);

                if (!runAndForget && isEnabled)
                {
                    await newBatchTask.WaitForTaskAsync();
                }
            }

            return batchTasks;
        }
    }
}
