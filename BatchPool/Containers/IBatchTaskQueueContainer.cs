namespace BatchPool
{
    /// <summary>
    /// An internal contract to improve consistency between all containers
    /// </summary>
    internal interface IBatchTaskQueueContainer
    {
        bool IsEmpty { get; }
        int GetPendingTaskCount();
        BatchPoolTask[] ToArray();
        bool TryDequeue(out BatchPoolTask? batchPoolTask);
    }
}
