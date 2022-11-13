using System.Collections;
using System.Collections.Generic;

namespace BatchPool.UnitTests.BasicTests.Helpers.ClassData
{
    internal class AddTasksInBatchData : IEnumerable<object[]>
    {
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
                                if (batchSize == 1
                                    && numberOfTests >= 1_000)
                                {
                                    // Remove very slow tests
                                    continue;
                                }

                                yield return new object[] { numberOfTests, batchSize, isEnabledState, runAndForgetState, taskType };
                            }
                        }
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
