using System;
using System.Collections.Generic;
using System.Linq;
using static BatchPool.UnitTests.Utilities.TestConstants;

namespace BatchPool.UnitTests.BasicTests.Helpers.ClassData
{
    internal static class SharedData
    {
        internal static readonly int[] NumbersOfTests = new int[] { 1, 10, 100, 1_000 };
        internal static readonly int[] BatchSizes = new int[] { 1, 10, 50, 100 };
        internal static readonly bool[] IsEnabledStates = new bool[] { true, false };
        internal static readonly bool[] RunAndForgetStates = new bool[] { true, false };
        internal static readonly List<TaskType> TaskTypes = Enum.GetValues(typeof(TaskType)).Cast<TaskType>().ToList();
    }
}
