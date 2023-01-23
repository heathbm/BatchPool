namespace BatchPool
{
    /// <summary>
    /// A generic task batching and management library.
    /// Tasks execution order is dynamically provided as tasks are added.
    /// </summary>
    public class BatchPoolDynamicContainer<T> : BatchPoolContainerBase
    {
        private readonly ConcurrentPriorityQueueContainer<T> _concurrentPriorityQueueContainer;

        public BatchPoolDynamicContainer(int batchSize, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
            : base(batchSize, isEnabled, inactiveTaskValidation, cancellationToken)
        {
            QueueContainer = _concurrentPriorityQueueContainer = new ConcurrentPriorityQueueContainer<T>();
        }

        public BatchPoolDynamicContainer(int batchSize, IComparer<T> comparer, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
            : base(batchSize, isEnabled, inactiveTaskValidation, cancellationToken)
        {
            QueueContainer = _concurrentPriorityQueueContainer = new ConcurrentPriorityQueueContainer<T>(comparer);
        }

        // Note: Add lots of overloads to make it easy for the consumer to quickly use this library.

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task, T priority) =>
            AddTask(task, priority);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task, T priority, Task callback, bool waitForCallback = false) =>
            AddTask(task, priority, taskCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task, T priority, Func<Task> callback, bool waitForCallback = false) =>
            AddTask(task, priority, functionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task, T priority, Action callback, bool waitForCallback = false) =>
            AddTask(task, priority, actionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function, T priority) =>
            AddFunc(function, priority);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function, T priority, Task callback, bool waitForCallback = false) =>
            AddFunc(function, priority, taskCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function, T priority, Func<Task> callback, bool waitForCallback = false) =>
            AddFunc(function, priority, functionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function, T priority, Action callback, bool waitForCallback = false) =>
            AddFunc(function, priority, actionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action, T priority) =>
            AddTask(new Task(action), priority);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action, T priority, Task callback, bool waitForCallback = false) =>
            AddTask(new Task(action), priority, taskCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action, T priority, Func<Task> callback, bool waitForCallback = false) =>
            AddTask(new Task(action), priority, functionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action, T priority, Action callback, bool waitForCallback = false) =>
            AddTask(new Task(action), priority, actionCallback: callback, waitForCallback: waitForCallback);

        private BatchTask AddTask(Task task, T priority, Task? taskCallback = null, Func<Task>? functionCallback = null, Action? actionCallback = null, bool waitForCallback = false)
        {
            BatchTask batchTask = GetTask(task, taskCallback, functionCallback, actionCallback, waitForCallback);
            _concurrentPriorityQueueContainer.Enqueue(batchTask, priority);
            Update(batchTask);
            return batchTask;
        }

        private BatchFunction AddFunc(Func<Task> function, T priority, Task? taskCallback = null, Func<Task>? functionCallback = null, Action? actionCallback = null, bool waitForCallback = false)
        {
            BatchFunction batchTask = GetFunc(function, taskCallback, functionCallback, actionCallback, waitForCallback);
            _concurrentPriorityQueueContainer.Enqueue(batchTask, priority);
            Update(batchTask);
            return batchTask;
        }
    }
}
