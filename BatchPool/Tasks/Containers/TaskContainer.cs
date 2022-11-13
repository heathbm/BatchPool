namespace BatchPool.Tasks.Containers
{
    internal struct TaskContainer : ITaskContainer
    {
        internal TaskContainer(Task task)
        {
            Task = task;
        }

        public Task? Task { get; set; }
    }
}
