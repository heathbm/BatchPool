using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static BatchPool.UnitTests.Utilities.TestConstants;

namespace BatchPool.UnitTests.BasicTests.Helpers.ClassData
{
    public class AddTasksIndividuallyData : IEnumerable<object[]>
    {
        private static readonly List<CallBackType> CallBackTypes = Enum.GetValues(typeof(CallBackType)).Cast<CallBackType>().ToList();
        private static readonly bool[] WaitForCallBackStates = new bool[] { true, false };

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var numberOfTests in SharedData.NumbersOfTests)
            {
                foreach (var batchSize in SharedData.BatchSizes)
                {
                    foreach (var isEnabledState in SharedData.IsEnabledStates)
                    {
                        foreach (var runAndForgetState in SharedData.RunAndForgetStates)
                        {
                            foreach (var taskType in SharedData.TaskTypes)
                            {
                                foreach (var callBackType in CallBackTypes)
                                {
                                    foreach (var waitForCallBackState in WaitForCallBackStates)
                                    {
                                        if (batchSize == 1)
                                        {
                                            // Remove very slow tests
                                            continue;
                                        }

                                        yield return new object[] { numberOfTests, batchSize, isEnabledState, runAndForgetState, taskType, callBackType, waitForCallBackState };
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
