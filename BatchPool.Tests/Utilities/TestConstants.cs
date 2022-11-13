namespace BatchPool.UnitTests.Utilities
{
    public static class TestConstants
    {
        public const int NumberOfTests = 1;

        public enum TaskType
        {
            Task,
            WrappedTask,
            AsyncFunc,
            Action
        }

        public enum CallBackType
        {
            None,
            Task,
            WrappedTask,
            AsyncFunc,
            Action
        }
    }
}
