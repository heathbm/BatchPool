using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.UnitTests.Scenarios
{
    public static class RunningTasksThrowException
    {
        [Fact]
        public static void RunningTask_WithRunningTaskValidation_ThrowsException()
        {
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true, inactiveTaskValidation: true);

            var task = new Task(() =>
            {
                Task.Delay(500).GetAwaiter().GetResult();
            });

            task.Start();
            Assert.Throws<ArgumentException>(() => batchPool.Add(task));
        }

        [Fact]
        public static void RunningTasksFirst_WithRunningTaskValidation_ThrowsException()
        {
            int numberOfTasks = 100;
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false, inactiveTaskValidation: true);
            var batchTasks = new List<Task>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add((new Task(() => Task.Delay(500).GetAwaiter().GetResult())));
            }

            batchTasks.First().Start();

            Assert.Throws<ArgumentException>(() => batchPool.Add(batchTasks));
        }

        [Fact]
        public static void RunningTasksLast_WithRunningTaskValidation_ThrowsException()
        {
            int numberOfTasks = 100;
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false, inactiveTaskValidation: true);
            var batchTasks = new List<Task>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add((new Task(() => Task.Delay(500).GetAwaiter().GetResult())));
            }

            batchTasks.First().Start();

            Assert.Throws<ArgumentException>(() => batchPool.Add(batchTasks));
        }

        [Fact]
        public static void RunningTasksMiddle_WithRunningTaskValidation_ThrowsException()
        {
            int numberOfTasks = 100;
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false, inactiveTaskValidation: true);
            var batchTasks = new List<Task>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add((new Task(() => Task.Delay(500).GetAwaiter().GetResult())));
            }

            var middleIndex = numberOfTasks / 2;
            batchTasks[middleIndex].Start();

            Assert.Throws<ArgumentException>(() => batchPool.Add(batchTasks));
        }

        [Fact]
        public static void RunningTask_WithoutRunningTaskValidation_DoesNotThrowException()
        {
            int batchSize = 1;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: true, inactiveTaskValidation: false);

            var task = new Task(() =>
            {
                Task.Delay(500).GetAwaiter().GetResult();
            });

            task.Start();
            var batchTask = batchPool.Add(task);
            Assert.NotNull(batchTask);
        }

        [Fact]
        public static void RunningTasksFirst_WithoutRunningTaskValidation_DoesNotThrowException()
        {
            int numberOfTasks = 100;
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false, inactiveTaskValidation: false);
            var batchTasks = new List<Task>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add((new Task(() => Task.Delay(500).GetAwaiter().GetResult())));
            }

            batchTasks.First().Start();

            var batchTask = batchPool.Add(batchTasks);
            Assert.NotNull(batchTask);
        }

        [Fact]
        public static void RunningTasksLast_WithoutRunningTaskValidation_DoesNotThrowException()
        {
            int numberOfTasks = 100;
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false, inactiveTaskValidation: false);
            var batchTasks = new List<Task>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add((new Task(() => Task.Delay(500).GetAwaiter().GetResult())));
            }

            batchTasks.First().Start();

            var batchTask = batchPool.Add(batchTasks);
            Assert.NotNull(batchTask);
        }

        [Fact]
        public static void RunningTasksMiddle_WithoutRunningTaskValidation_DoesNotThrowException()
        {
            int numberOfTasks = 100;
            int batchSize = 5;
            var batchPool = new BatchPoolContainer(batchSize, isEnabled: false, inactiveTaskValidation: false);
            var batchTasks = new List<Task>();

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchTasks.Add((new Task(() => Task.Delay(500).GetAwaiter().GetResult())));
            }

            var middleIndex = numberOfTasks / 2;
            batchTasks[middleIndex].Start();

            var batchTask = batchPool.Add(batchTasks);
            Assert.NotNull(batchTask);
        }
    }
}
