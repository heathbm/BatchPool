namespace BatchPool.Containers
{
    /// <summary>
    /// A comparer used by PriorityQueues to dynamically order BatchPoolTasks wrapped in PriorityStateContainers
    /// </summary>
    internal class PriorityStateContainerComparer<T> : IComparer<PriorityStateContainer<T>>
        where T : IComparable<T>
    {
        private readonly IComparer<T> _comparer;

        public PriorityStateContainerComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(PriorityStateContainer<T>? x, PriorityStateContainer<T>? y)
        {
            return _comparer.Compare(x!.Priority, y!.Priority);
        }
    }
}
