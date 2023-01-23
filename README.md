<img src=".images/packageIcon.png" alt="drawing" width="160"/>

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
- [Dynamic ordering](#dynamic-ordering)
- [Dynamic custom ordering](#dynamic-custom-ordering)
- [Updatable dynamic ordering](#updatable-dynamic-ordering)
- [Updatable dynamic custom ordering](#updatable-dynamic-custom-ordering)
- [Factory](#factory)
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

bool isCanceled = task.IsCanceled;
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
// All pending tasks will be Canceled
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

### Dynamic ordering

`int` order example:
```C#
int batchSize = 1;
// The int type provided as the generic parameter will provide dynamic ordering using the default .NET comparer. Order exection will be ascending (smallest to largest).
BatchPoolDynamicContainer<int> batchPool = new BatchPoolDynamicContainer<int>(batchSize, isEnabled: false);

var task1 = new Task(() => Console.WriteLine("Hello #1"));
var batchTask1 = batchPool.Add(task1, priority: 1);

var task2 = new Task(() => Console.WriteLine("Hello #2"));
var batchTask2 = batchPool.Add(task2, 2);

var task3 = new Task(() => Console.WriteLine("Hello #3"));
var batchTask3 = batchPool.Add(task3, 3);

await batchPool.ResumeAndWaitForAllAsync();
```

`string` order example:
```C#
BatchPoolDynamicContainer<string> batchPool = new BatchPoolDynamicContainer<string>(batchSize, isEnabled: false);

var task1 = new Task(() => Console.WriteLine("Hello #1"));
var batchTask1 = batchPool.Add(task1, priority: "a");

var task2 = new Task(() => Console.WriteLine("Hello #2"));
var batchTask2 = batchPool.Add(task2, "b");

var task3 = new Task(() => Console.WriteLine("Hello #3"));
var batchTask3 = batchPool.Add(task3, "c");

await batchPool.ResumeAndWaitForAllAsync();
```

`enum` order example:
```C#
public enum TestEnum
{
    Critical = 0,
    High = 1,
    Medium = 2,
    Low = 3
}

int batchSize = 1;
var batchPool = new BatchPoolDynamicContainer<TestEnum>(batchSize, isEnabled: false);

var task1 = new Task(() => executionOrderTracker.Add(TestEnum.High.ToString()));
var batchTask1 = batchPool.Add(task1, TestEnum.High);

var task2 = new Task(() => executionOrderTracker.Add(TestEnum.Medium.ToString()));
var batchTask2 = batchPool.Add(task2, TestEnum.Medium);

var task3 = new Task(() => executionOrderTracker.Add(TestEnum.Low.ToString()));
var batchTask3 = batchPool.Add(task3, TestEnum.Low);

await batchPool.ResumeAndWaitForAllAsync();
```

### Dynamic custom ordering

Custom ordering simply by passing a new IComparer<T>:

```C#
// Reverse the default order
public class StringReverseComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        return y!.CompareTo(x!);
    }
}

BatchPoolDynamicContainer<string> batchPool = new BatchPoolDynamicContainer<string>(batchSize, new StringReverseComparer(), isEnabled: false);
```

### Updatable dynamic ordering

```C#
int batchSize = 1;
// Use BatchPoolUpdatableDynamicContainer instead of BatchPoolDynamicContainer
BatchPoolUpdatableDynamicContainer<int> batchPool = new BatchPoolUpdatableDynamicContainer<int>(batchSize, isEnabled: false);

var task1 = new Task(() => Console.WriteLine("Hello #1"));
var batchTask1 = batchPool.Add(task1, priority: 1);

var task2 = new Task(() => Console.WriteLine("Hello #2"));
var batchTask2 = batchPool.Add(task2, 2);

var task3 = new Task(() => Console.WriteLine("Hello #3"));
var batchTask3 = batchPool.Add(task3, 3);

// batchTask1 will now execute last instead of first
batchPool.UpdatePriority(batchTask1, 4);

await batchPool.ResumeAndWaitForAllAsync();
```

### Updatable dynamic custom ordering

Custom ordering simply by passing a new IComparer<T>:

```C#
// Reverse the default order
public class StringReverseComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        return y!.CompareTo(x!);
    }
}

BatchPoolUpdatableDynamicContainer<string> batchPool = new BatchPoolUpdatableDynamicContainer<string>(batchSize, new StringReverseComparer(), isEnabled: false);
```

**Technical Note**: More checks are required for `BatchPoolUpdatableDynamicContainer` over the `BatchPoolDynamicContainer`. However, the performance impact should not be noticeable as they occur at O(1). `UpdatePriority` does not modify the existing data structure, but instead allows the container to check if a priority is current and valid or a new has been added.

### Factory

The `BatchPoolFactory` simply provides a quick way to instantiate a new BatchPoolContainer:

```C#
// Default queue
BatchPoolContainer batchPool = BatchPoolFactory.GetQueueBatchPool(batchSize, isEnabled: false);

// Dynamic ordering
BatchPoolDynamicContainer<int> batchPool = BatchPoolFactory.GetDynamicallyOrderedBatchPool<int>(batchSize, isEnabled: false);

// Dynamic ordering and update existing tasks priority
BatchPoolUpdatableDynamicContainer<int> batchPool = BatchPoolFactory.GetUpdatableDynamicallyOrderedBatchPool<int>(batchSize, isEnabled: false);
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
BatchPoolContainerManager<BatchPoolContainer> batchPoolContainerManager = new BatchPoolContainerManager<BatchPoolContainer>();

// Create and register a BatchPool
BatchPoolContainer batchPoolContainer = new BatchPoolContainer(batchSize: 5, isEnabled: true);
batchPoolContainer = batchPoolContainerManager.RegisterBatchPool("UniqueBatchPoolName", batchPoolContainer);

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
    .ConfigureServices((_, services) => services.AddSingleton<BatchPoolContainerManager<BatchPoolContainer>>())
    .Build();

using IServiceScope serviceScope = hostBuilder.Services.CreateScope();
IServiceProvider serviceProvider = serviceScope.ServiceProvider;

BatchPoolContainerManager<BatchPoolContainer> batchPoolContainerManager = serviceProvider.GetRequiredService<BatchPoolContainerManager<BatchPoolContainer>>();
```
