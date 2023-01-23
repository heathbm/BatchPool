namespace BatchPool
{
    /// <summary>
    /// A generic task batching and management library.
    /// Tasks are executed in the order in which they are received.
    /// </summary>
    public class BatchPoolContainer : BatchPoolContainerBase
    {
        private readonly ConcurrentQueueContainer _queueContainer;

        public BatchPoolContainer(int batchSize, bool isEnabled = true, bool inactiveTaskValidation = false, CancellationToken cancellationToken = default)
            : base(batchSize, isEnabled, inactiveTaskValidation, cancellationToken)
        {
            QueueContainer = _queueContainer = new ConcurrentQueueContainer();
        }

        // Note: Add lots of overloads to make it easy for the consumer to quickly use this library.

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task) =>
            AddTask(task);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task, Task callback, bool waitForCallback = false) =>
            AddTask(task, taskCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task, Func<Task> callback, bool waitForCallback = false) =>
            AddTask(task, functionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Task task, Action callback, bool waitForCallback = false) =>
            AddTask(task, actionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function) =>
            AddFunc(function);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function, Task callback, bool waitForCallback = false) =>
            AddFunc(function, taskCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function, Func<Task> callback, bool waitForCallback = false) =>
            AddFunc(function, functionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Func<Task> function, Action callback, bool waitForCallback = false) =>
            AddFunc(function, actionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action) =>
            AddTask(new Task(action));

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action, Task callback, bool waitForCallback = false) =>
            AddTask(new Task(action), taskCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action, Func<Task> callback, bool waitForCallback = false) =>
            AddTask(new Task(action), functionCallback: callback, waitForCallback: waitForCallback);

        /// <summary>
        /// Add a task to the BatchPoolContainer. The task will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public BatchPoolTask Add(Action action, Action callback, bool waitForCallback = false) =>
            AddTask(new Task(action), actionCallback: callback, waitForCallback: waitForCallback);
        /// <summary>
        /// Add a tasks to the BatchPoolContainer. The tasks will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public List<BatchPoolTask> Add(IEnumerable<Task> tasks)
        {
            List<BatchPoolTask> batchTasks = new();

            foreach (Task task in tasks)
            {
                batchTasks.Add(AddTask(task));
            }

            return batchTasks;
        }

        /// <summary>
        /// Add a tasks to the BatchPoolContainer. The tasks will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public List<BatchPoolTask> Add(IEnumerable<Func<Task>> functions)
        {
            List<BatchPoolTask> batchTasks = new();

            foreach (Func<Task> function in functions)
            {
                batchTasks.Add(AddFunc(function));
            }

            return batchTasks;
        }

        /// <summary>
        /// Add a tasks to the BatchPoolContainer. The tasks will automatically start when the it can if the BatchPoolContainer is enabled.
        /// </summary>
        public List<BatchPoolTask> Add(IEnumerable<Action> functions)
        {
            List<BatchPoolTask> batchTasks = new();

            foreach (Action function in functions)
            {
                batchTasks.Add(AddTask(new Task(function)));
            }

            return batchTasks;
        }

        private BatchTask AddTask(Task task, Task? taskCallback = null, Func<Task>? functionCallback = null, Action? actionCallback = null, bool waitForCallback = false)
        {
            BatchTask batchTask = GetTask(task, taskCallback, functionCallback, actionCallback, waitForCallback);
            _queueContainer.Enqueue(batchTask);
            Update(batchTask);
            return batchTask;
        }

        private BatchFunction AddFunc(Func<Task> function, Task? taskCallback = null, Func<Task>? functionCallback = null, Action? actionCallback = null, bool waitForCallback = false)
        {
            BatchFunction batchTask = GetFunc(function, taskCallback, functionCallback, actionCallback, waitForCallback);
            _queueContainer.Enqueue(batchTask);
            Update(batchTask);
            return batchTask;
        }
    }
}
