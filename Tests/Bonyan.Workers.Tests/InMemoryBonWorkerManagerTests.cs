using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Bonyan.Tests.Workers.Mock;
using Bonyan.Workers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Bonyan.Tests.Workers
{
    public class InMemoryBonWorkerManagerTests
    {
        private readonly IServiceCollection _services;
        private readonly IServiceProvider _serviceProvider;

        public InMemoryBonWorkerManagerTests()
        {
            _services = new ServiceCollection();
            _services.AddTransient<TestWorkerA>();
            _services.AddTransient<TestWorkerB>();
            _serviceProvider = _services.BuildServiceProvider();
        }

        [Fact]
        public void Enqueue_Should_Add_Worker_To_Queue()
        {
            // Arrange
            var manager = new InMemoryBonWorkerManager(_serviceProvider);

            // Act
            manager.Enqueue<TestWorkerA>();

            // Internal verification (using reflection since _backgroundJobs is private)
            var backgroundJobsField = typeof(InMemoryBonWorkerManager)
                .GetField("_backgroundJobs", BindingFlags.NonPublic | BindingFlags.Instance);
            var backgroundJobs = (ConcurrentQueue<Type>)backgroundJobsField.GetValue(manager);

            // Assert
            Assert.Single(backgroundJobs);
            Assert.Contains(typeof(TestWorkerA), backgroundJobs);
        }

        [Fact]
        public void ScheduleRecurring_Should_Add_Worker_To_RecurringJobs()
        {
            // Arrange
            var manager = new InMemoryBonWorkerManager(_serviceProvider);

            // Act
            manager.ScheduleRecurring<TestWorkerA>("* * * * *"); // Every minute

            // Internal verification
            var recurringJobsField = typeof(InMemoryBonWorkerManager)
                .GetField("_recurringJobs", BindingFlags.NonPublic | BindingFlags.Instance);

            var recurringJobs = recurringJobsField.GetValue(manager) as System.Collections.IDictionary;

            // Assert
            Assert.NotNull(recurringJobs);
            Assert.Equal(1, recurringJobs.Count);
            Assert.Contains(typeof(TestWorkerA).FullName, recurringJobs.Keys.Cast<string>());
        }

        [Fact]
        public void CancelRecurring_Should_Remove_Worker_From_RecurringJobs()
        {
            // Arrange
            var manager = new InMemoryBonWorkerManager(_serviceProvider);
            var jobId = "TestJob";

            manager.ScheduleRecurring<TestWorkerA>("* * * * *", jobId);

            // Act
            manager.CancelRecurring(jobId);

            // Internal verification
            var recurringJobsField = typeof(InMemoryBonWorkerManager)
                .GetField("_recurringJobs", BindingFlags.NonPublic | BindingFlags.Instance);
            var recurringJobs = (ConcurrentDictionary<string, object>)recurringJobsField.GetValue(manager);

            // Assert
            Assert.Empty(recurringJobs);
        }

        [Fact]
        public async Task Enqueued_Worker_Should_Be_Executed()
        {
            // Arrange
            var workerMock = new Mock<IBonWorker>();
            workerMock.Setup(w => w.ExecuteAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var services = new ServiceCollection();

            // Register the mock type with the service provider
            services.AddTransient(workerMock.Object.GetType(), provider => workerMock.Object);

            var serviceProvider = services.BuildServiceProvider();

            var manager = new InMemoryBonWorkerManager(serviceProvider);

            // Act
            manager.Enqueue(workerMock.Object.GetType());

            // Wait some time for the background task to process the queue
            await Task.Delay(2000);

            // Assert
            workerMock.Verify(w => w.ExecuteAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Worker_Execution_Exception_Should_Not_Stop_Manager()
        {
            // Arrange
            var tcs1 = new TaskCompletionSource<bool>();
            var tcs2 = new TaskCompletionSource<bool>();

            var workerMock = new Mock<IBonWorker>();
            workerMock.Setup(w => w.ExecuteAsync(It.IsAny<CancellationToken>()))
                .Callback(() => tcs1.SetResult(true))
                .Returns(() => Task.FromException(new Exception("Test exception")));

            var workerMock2 = new Mock<IBonWorker>();
            workerMock2.Setup(w => w.ExecuteAsync(It.IsAny<CancellationToken>()))
                .Callback(() => tcs2.SetResult(true))
                .Returns(Task.CompletedTask);

            var services = new ServiceCollection();

            // Register workers directly as IBonWorker
            services.AddSingleton<IBonWorker>(workerMock.Object);
            services.AddSingleton<IBonWorker>(workerMock2.Object);

            var serviceProvider = services.BuildServiceProvider();

            // Reduce background processing delay in the manager for faster testing
            var manager = new InMemoryBonWorkerManager(serviceProvider);

            // Act
            // Enqueue the first worker that throws an exception
            manager.Enqueue(typeof(IBonWorker));

            // Wait for the worker's ExecuteAsync to be called
            await Task.WhenAny(tcs1.Task, Task.Delay(5000));

            // Assert that the first worker's ExecuteAsync was called
            Assert.True(tcs1.Task.IsCompleted, "First worker's ExecuteAsync was not called.");

            // Enqueue the second worker to verify the manager is still running
            manager.Enqueue(typeof(IBonWorker));

            // Wait for the second worker's ExecuteAsync to be called
            await Task.WhenAny(tcs2.Task, Task.Delay(5000));

            // Assert that the second worker's ExecuteAsync was called
            Assert.True(tcs2.Task.IsCompleted, "Second worker's ExecuteAsync was not called.");
        }


        [Fact]
        public void Enqueue_Should_Throw_On_Null_JobType()
        {
            // Arrange
            var manager = new InMemoryBonWorkerManager(_serviceProvider);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => manager.Enqueue(null));
        }

        [Fact]
        public void Enqueue_Should_Throw_On_Invalid_Worker_Type()
        {
            // Arrange
            var manager = new InMemoryBonWorkerManager(_serviceProvider);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => manager.Enqueue(typeof(string)));
            Assert.Contains("does not implement IBonWorker", ex.Message);
        }

        [Fact]
        public void Dispose_Should_Cancel_Tasks()
        {
            // Arrange
            var manager = new InMemoryBonWorkerManager(_serviceProvider);

            // Act
            manager.Dispose();

            // Assert
            // Internal verification
            var ctsField = typeof(InMemoryBonWorkerManager)
                .GetField("_cts", BindingFlags.NonPublic | BindingFlags.Instance);
            var cts = (CancellationTokenSource)ctsField.GetValue(manager);

            Assert.True(cts.IsCancellationRequested);
        }
    }
}
