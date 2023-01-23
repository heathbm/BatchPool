namespace BatchPool.Containers
{
    /// <summary>
    /// An internal container to wrap a BatchPoolTask to provide a unique assignment of priority and the state of the priority.
    /// </summary>
    internal class PriorityStateContainer<T> : IComparable<PriorityStateContainer<T>>
        where T : IComparable<T>
    {
        internal PriorityStateContainer(T priority)
        {
            Priority = priority;
        }

        internal T Priority { get; set; }
        internal bool IsDisabled { get; private set; }

        public int CompareTo(PriorityStateContainer<T>? other)
        {
            return Priority.CompareTo(other!.Priority);
        }

        internal void Disable()
        {
            IsDisabled = true;
        }
    }
}
