# BatchPool

[![Build & Tests](https://github.com/heathbm/BatchPool/actions/workflows/dotnet.yml/badge.svg)](https://github.com/heathbm/BatchPool/actions/workflows/dotnet.yml)

The one-stop generic task batching and management library.  
Contributions are welcome to add features, flexibility, performance test coverage...  

# Features

- Very low overhead, flexible, high test coverage, generic...
- Tasks are executed in the order in which they are receieved  
- [Supports Task, Func and Action](#supports-task--func-and-action)
- [BatchPool states: Enabled / Paused](#batchpool-states--enabled---paused)
- [Dynamic batch size: update the size of the BatchPool](#dynamic-batch-size--update-the-size-of-the-batchpool)
- [Callbacks: supports Task, Func and Action](#callbacks--supports-task--func-and-action)
- [Check the state of a task](#check-the-state-of-a-task)
- [Task Cancellation](#task-cancellation)
- [BatchPool Cancellation](#batchpool-cancellation)
- [Adding tasks in batch](#adding-tasks-in-batch)
- [Waiting for tasks to finish](#waiting-for-tasks-to-finish)
- [BatchPoolManagers](#batchpoolmanagers)
- [BatchPoolManager with DI](#batchpoolmanager-with-di)

### Supports Task, Func and Action

```C#
// Set batchSize to configure the maximum number of tasks that can be run concurrently
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: true);

// Task
Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchTask task = batchPool.Add(aTask);
await task.WaitForTaskAsync();

// Func
Func<Task> aFunc = async () => Console.WriteLine("Hello");
BatchTask func = batchPool.Add(aFunc);
await func.WaitForTaskAsync();

// Action
Action anAction = () => Console.WriteLine("Hello");
BatchTask action = batchPool.Add(anAction);
await action.WaitForTaskAsync();
```

### BatchPool states: Enabled / Paused

```C#
// Set isEnabled to configure the state of the BatchPool at initialization
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: false);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchTask task = batchPool.Add(aTask);
await task.WaitForTaskAsync();

// Resume and forget
batchPool.ResumeAndForget();
// Or resume and wait for all task to finish
await batchPool.ResumeAndWaitForAllAsync();

// Then pause again to prevent new pending tasks to run
batchPool.Pause();
```

### Dynamic batch size: update the size of the BatchPool

```C#
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: false);

// Increase or reduce the capacity and wait for it to finish updating. (The BatchPool will need to wait if a reduction is requested while it is currently processing)
await batchPool.UpdateCapacityAsync(10);
// Perform the same operation in the background
batchPool.UpdateCapacityAndForget(10);
```

### Callbacks: supports Task, Func and Action

```C#
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: true);

Task aTask = new Task(() => Console.WriteLine("Hello"));
// The callback will run as soon as the main task completes
Task aCallbackTask = new Task(() => Console.WriteLine("Hello"));

BatchTask task = batchPool.Add(aTask, aCallbackTask);
await task.WaitForTaskAsync();
```

### Check the state of a task

```C#
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: true);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchTask task = batchPool.Add(aTask);

bool isCancelled = task.IsCancelled;
bool isCompleted = task.IsCompleted;
```

### Task Cancellation

```C#
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: true);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchTask task = batchPool.Add(aTask);

// Attempt to cancel
bool didCancel = task.Cancel();
// Attempt to cancel all pending tasks (pending = tasks that have not yet started processing due to the batch size, or the paused state of the BatchPool)
batchPool.RemoveAndCancelPendingTasks();
```

### BatchPool Cancellation

```C#
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: true, cancellationToken: cancellationTokenSource.Token);
// All pending tasks will be cancelled
cancellationTokenSource.Cancel();
```

### Adding tasks in batch

```C#
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: true);

Task aTask1 = new Task(() => Console.WriteLine("Hello"));
Task aTask2 = new Task(() => Console.WriteLine("Hello"));
List<Task> listOfTasks = new List<Task>() { aTask1, aTask2 };

ICollection<BatchTask> tasks = batchPool.Add(listOfTasks);
```

### Waiting for tasks to finish

```C#
BatchPool batchPool = new BatchPool(batchSize: 5, isEnabled: true);

Task aTask1 = new Task(() => Console.WriteLine("Hello"));
Task aTask2 = new Task(() => Console.WriteLine("Hello"));
List<Task> listOfTasks = new List<Task>() { aTask1, aTask2 };

List<BatchTask> tasks = batchPool.Add(listOfTasks);

// Wait for each task individually
await tasks[0].WaitForTaskAsync();
await tasks[1].WaitForTaskAsync();

// Wait for all tasks to finish
await batchPool.WaitForAllAsync();
// With timeoutInMilliseconds
await batchPool.WaitForAllAsync(timeoutInMilliseconds: 100);
// With timeout
await batchPool.WaitForAllAsync(timeout: TimeSpan.FromMilliseconds(100));
// With cancellationToken
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
await batchPool.WaitForAllAsync(cancellationToken: cancellationTokenSource.Token);

// With cancellationToken and timeoutInMilliseconds/cancellationToken
await batchPool.WaitForAllAsync(timeoutInMilliseconds: 100, cancellationTokenSource.Token);
await batchPool.WaitForAllAsync(timeout: TimeSpan.FromMilliseconds(100), cancellationTokenSource.Token);
```

### BatchPoolManagers

```C#
BatchPoolManager batchPoolManager = new BatchPoolManager();

// Create and register a BatchPool
BatchPool batchPool = batchPoolManager.CreateAndRegisterBatch("UniqueBatchPoolName", batchSize: 5, isEnabled: true);

// Retrieve the BatchPool
bool isFound = batchPoolManager.TryGetBatchPool("UniqueBatchPoolName", out BatchPool retrievedBatchPool);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchTask task = batchPool.Add(aTask);

// Wait for all tasks in all BatchPools to finish
await batchPoolManager.WaitForAllBatchPools();
```

### BatchPoolManager with DI

```C#
IHost hostBuilder = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) => services.AddSingleton<BatchPoolManager>())
    .Build();

using IServiceScope serviceScope = hostBuilder.Services.CreateScope();
IServiceProvider serviceProvider = serviceScope.ServiceProvider;

BatchPoolManager batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();
```
