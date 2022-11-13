namespace BatchPool.Tasks.Containers
{
    internal interface ITaskContainer
    {
        Task? Task { get; set; }
    }
}
