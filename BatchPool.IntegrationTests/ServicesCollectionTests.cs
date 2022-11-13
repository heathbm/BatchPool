using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BatchPool.IntegrationTests
{
    public static class ServicesCollectionTests
    {
        [Fact]
        public static async Task HostAddSingleton_CreateAndRunTasksThenWaitForAll_IsSuccessful()
        {
            int numberOfTasks = 1000;
            int localProgress = 0;
            int taskProgress = 0;
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddSingleton<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchPool.Add(new Task(() => Interlocked.Increment(ref taskProgress)));
                localProgress++;
            }

            Assert.Equal(numberOfTasks, localProgress);

            await batchPool.WaitForAllAsync();

            Assert.Equal(numberOfTasks, taskProgress);
        }

        [Fact]
        public static async Task HostAddScoped_CreateAndRunTasksThenWaitForAll_IsSuccessful()
        {
            int numberOfTasks = 1000;
            int localProgress = 0;
            int taskProgress = 0;
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddScoped<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchPool.Add(new Task(() => Interlocked.Increment(ref taskProgress)));
                localProgress++;
            }

            Assert.Equal(numberOfTasks, localProgress);

            await batchPool.WaitForAllAsync();

            Assert.Equal(numberOfTasks, taskProgress);
        }

        [Fact]
        public static async Task HostAddTransient_CreateAndRunTasksThenWaitForAll_IsSuccessful()
        {
            int numberOfTasks = 1000;
            int localProgress = 0;
            int taskProgress = 0;
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddTransient<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            for (int taskIndex = 0; taskIndex < numberOfTasks; taskIndex++)
            {
                batchPool.Add(new Task(() => Interlocked.Increment(ref taskProgress)));
                localProgress++;
            }

            Assert.Equal(numberOfTasks, localProgress);

            await batchPool.WaitForAllAsync();

            Assert.Equal(numberOfTasks, taskProgress);
        }

        [Fact]
        public static void HostAddSingleton_DuplicateName_ReturnsTheSameBatchPool()
        {
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddSingleton<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);
            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            Assert.Equal(batchPool1, batchPool2);
        }

        [Fact]
        public static void HostAddScoped_DuplicateName_ReturnsTheSameBatchPool()
        {
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddScoped<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);
            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            Assert.Equal(batchPool1, batchPool2);
        }

        [Fact]
        public static void HostAddTransient_DuplicateName_ReturnsTheSameBatchPool()
        {
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddTransient<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);
            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            Assert.Equal(batchPool1, batchPool2);
        }

        [Fact]
        public static void HostAddSingleton_DuplicateNameInDifferentScopes_ReturnsTheSameBatchPool()
        {
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddSingleton<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            using var serviceScope2 = hostBuilder.Services.CreateScope();
            serviceProvider = serviceScope2.ServiceProvider;

            batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            Assert.Equal(batchPool1, batchPool2);
        }

        [Fact]
        public static void HostAddScoped_DuplicateNameInDifferentScopes_ReturnsTheDifferentBatchPools()
        {
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddScoped<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            using var serviceScope2 = hostBuilder.Services.CreateScope();
            serviceProvider = serviceScope2.ServiceProvider;

            batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            Assert.NotEqual(batchPool1, batchPool2);
        }

        [Fact]
        public static void HostAddTransient_DuplicateNameInDifferentScopes_ReturnsTheDifferentBatchPools()
        {
            int batchSize = 5;

            var hostBuilder = Host.CreateDefaultBuilder()
                       .ConfigureServices((_, services) => services.AddTransient<BatchPoolManager>())
                       .Build();

            using var serviceScope = hostBuilder.Services.CreateScope();
            var serviceProvider = serviceScope.ServiceProvider;

            var batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool1 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            using var serviceScope2 = hostBuilder.Services.CreateScope();
            serviceProvider = serviceScope2.ServiceProvider;

            batchPoolManager = serviceProvider.GetRequiredService<BatchPoolManager>();

            var batchPool2 = batchPoolManager.CreateAndRegisterBatch("123", batchSize);

            Assert.NotEqual(batchPool1, batchPool2);
        }
    }
}