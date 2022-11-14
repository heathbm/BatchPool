# BatchPool

[![Build & Tests](https://github.com/heathbm/BatchPool/actions/workflows/dotnet.yml/badge.svg)](https://github.com/heathbm/BatchPool/actions/workflows/dotnet.yml)  
[![NuGet Version](https://img.shields.io/nuget/v/BatchPool)](https://www.nuget.org/packages/BatchPool)  
[![NuGet Downlaods](https://img.shields.io/nuget/dt/BatchPool?label=nuget%20downloads)](https://www.nuget.org/packages/BatchPool)  
[![GitHub License](https://img.shields.io/github/license/heathbm/batchpool)](https://github.com/heathbm/BatchPool/blob/master/LICENSE)  

The one-stop generic task batching and management library.  
Contributions are welcome to add features, flexibility, performance test coverage...  

# Features

- Very low overhead, flexible, high test coverage, generic...
- Tasks are executed in the order in which they are received  
- [Supports Task, Func and Action](#supports-task--func-and-action)
- [BatchPoolContainer states: Enabled / Paused](#batchpoolcontainer-states--enabled---paused)
- [Dynamic batch size: update the size of the BatchPool](#dynamic-batch-size--update-the-size-of-the-batchpool)
- [Callbacks: supports Task, Func and Action](#callbacks--supports-task--func-and-action)
- [Check the state of a task](#check-the-state-of-a-task)
- [Task Cancellation](#task-cancellation)
- [batchPoolContainer Cancellation](#batchpoolcontainer-cancellation)
- [Adding tasks in batch](#adding-tasks-in-batch)
- [Waiting for tasks to finish](#waiting-for-tasks-to-finish)
- [BatchPoolContainerManager](#batchpoolcontainermanager)
- [BatchPoolContainerManager with DI](#batchpoolcontainermanager-with-di)

### Supports Task, Func and Action

```C#
// Set batchSize to configure the maximum number of tasks that can be run concurrently
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true);

// Task
Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchPoolTask task = batchPoolContainer.Add(aTask);
await task.WaitForTaskAsync();

// Func
Func<Task> aFunc = async () => Console.WriteLine("Hello");
BatchPoolTask func = batchPoolContainer.Add(aFunc);
await func.WaitForTaskAsync();

// Action
Action anAction = () => Console.WriteLine("Hello");
BatchPoolTask action = batchPoolContainer.Add(anAction);
await action.WaitForTaskAsync();
```

### BatchPoolContainer states: Enabled / Paused

```C#
// Set isEnabled to configure the state of the batchPoolContainer at initialization
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: false);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchPoolTask task = batchPoolContainer.Add(aTask);
await task.WaitForTaskAsync();

// Resume and forget
batchPoolContainer.ResumeAndForget();
// Or resume and wait for all task to finish
await batchPoolContainer.ResumeAndWaitForAllAsync();

// Then pause again to prevent new pending tasks to run
batchPoolContainer.Pause();
```

### Dynamic batch size: update the size of the BatchPool

```C#
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: false);

// Increase or reduce the capacity and wait for it to finish updating. (The batchPoolContainer will need to wait if a reduction is requested while it is currently processing)
await batchPoolContainer.UpdateCapacityAsync(10);
// Perform the same operation in the background
batchPoolContainer.UpdateCapacityAndForget(10);
```

### Callbacks: supports Task, Func and Action

```C#
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true);

Task aTask = new Task(() => Console.WriteLine("Hello"));
// The callback will run as soon as the main task completes
Task aCallbackTask = new Task(() => Console.WriteLine("Hello"));

BatchPoolTask task = batchPoolContainer.Add(aTask, aCallbackTask);
await task.WaitForTaskAsync();
```

### Check the state of a task

```C#
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchPoolTask task = batchPoolContainer.Add(aTask);

bool isCancelled = task.IsCancelled;
bool isCompleted = task.IsCompleted;
```

### Task Cancellation

```C#
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchPoolTask task = batchPoolContainer.Add(aTask);

// Attempt to cancel
bool didCancel = task.Cancel();
// Attempt to cancel all pending tasks (pending = tasks that have not yet started processing due to the batch size, or the paused state of the BatchPool)
batchPoolContainer.RemoveAndCancelPendingTasks();
```

### batchPoolContainer Cancellation

```C#
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true, cancellationToken: cancellationTokenSource.Token);
// All pending tasks will be cancelled
cancellationTokenSource.Cancel();
```

### Adding tasks in batch

```C#
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true);

Task aTask1 = new Task(() => Console.WriteLine("Hello"));
Task aTask2 = new Task(() => Console.WriteLine("Hello"));
List<Task> listOfTasks = new List<Task>() { aTask1, aTask2 };

ICollection<BatchPoolTask> tasks = batchPoolContainer.Add(listOfTasks);
```

### Waiting for tasks to finish

```C#
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true);

Task aTask1 = new Task(() => Console.WriteLine("Hello"));
Task aTask2 = new Task(() => Console.WriteLine("Hello"));
List<Task> listOfTasks = new List<Task>() { aTask1, aTask2 };

List<BatchPoolTask> tasks = batchPoolContainer.Add(listOfTasks);

// Wait for each task individually
await tasks[0].WaitForTaskAsync();
await tasks[1].WaitForTaskAsync();

// Wait for all tasks to finish
await batchPoolContainer.WaitForAllAsync();
// With timeoutInMilliseconds
await batchPoolContainer.WaitForAllAsync(timeoutInMilliseconds: 100);
// With timeout
await batchPoolContainer.WaitForAllAsync(timeout: TimeSpan.FromMilliseconds(100));
// With cancellationToken
CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
await batchPoolContainer.WaitForAllAsync(cancellationToken: cancellationTokenSource.Token);

// With cancellationToken and timeoutInMilliseconds/cancellationToken
await batchPoolContainer.WaitForAllAsync(timeoutInMilliseconds: 100, cancellationTokenSource.Token);
await batchPoolContainer.WaitForAllAsync(timeout: TimeSpan.FromMilliseconds(100), cancellationTokenSource.Token);
```

### BatchPoolContainerManager

```C#
BatchPoolContainerManager batchPoolContainerManager = new BatchPoolContainerManager();

// Create and register a BatchPool
batchPoolContainer batchPool = batchPoolContainerManager.CreateAndRegisterBatch("UniqueBatchPoolName", batchSize: 5, isEnabled: true);

// Retrieve the BatchPool
bool isFound = batchPoolContainerManager.TryGetBatchPool("UniqueBatchPoolName", out batchPoolContainer retrievedBatchPool);

Task aTask = new Task(() => Console.WriteLine("Hello"));
BatchPoolTask task = batchPoolContainer.Add(aTask);

// Wait for all tasks in all BatchPools to finish
await batchPoolContainerManager.WaitForAllBatchPools();
```

### BatchPoolContainerManager with DI

```C#
IHost hostBuilder = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) => services.AddSingleton<BatchPoolContainerManager>())
    .Build();

using IServiceScope serviceScope = hostBuilder.Services.CreateScope();
IServiceProvider serviceProvider = serviceScope.ServiceProvider;

BatchPoolContainerManager batchPoolContainerManager = serviceProvider.GetRequiredService<BatchPoolContainerManager>();
```
