# Job Module Guide

The **Bonyan Job Module** is a powerful addition to the Bonyan Modular Application Framework, allowing developers to seamlessly integrate background jobs into their modular applications. This guide provides comprehensive, step-by-step instructions for installing the Job Module, defining jobs, and leveraging cron-based scheduling to handle repetitive tasks effectively.

## Table of Contents
- [Installation](#installation)
- [Step-by-Step Job Creation](#step-by-step-job-creation)
    - [Add Job Module Dependency](#add-job-module-dependency)
    - [Creating a Job](#creating-a-job)
    - [Registering the Job in a Module](#registering-the-job-in-a-module)
    - [Background Job vs. Cron Job](#background-job-vs-cron-job)
- [Job Examples](#job-examples)
    - [Basic Job Example](#basic-job-example)
    - [Cron Job Example](#cron-job-example)
- [Summary](#summary)

## Installation

To install the **Bonyan Job Module**, add the package to your .NET Core project using the following command:

```bash
dotnet add package Bonyan.Job
```

This command will include the necessary libraries and tools for integrating jobs into your application modules.

## Step-by-Step Job Creation

### Add Job Module Dependency

To use the Job Module, your main module must declare a dependency on `BonyanJobModule`. This ensures that all necessary services and configurations are available for managing jobs.

Here is how to declare the dependency in your module:

```csharp
[DependOn(typeof(BonyanJobModule))]
public class MyMainModule : Module
{
    public override Task OnConfigureAsync(ModularityContext context)
    {
        // Adding a job to the context
        context.AddJob<MyJob>();
        return base.OnConfigureAsync(context);
    }
}
```

In this example, the `MyMainModule` depends on `BonyanJobModule`, allowing it to manage job-related configurations and services. You can then add jobs to your context using `context.AddJob<TJob>()`.

### Creating a Job

Jobs in Bonyan are background processes that can perform recurring or long-running tasks. To define a job, you need to implement the `IJob` interface. This interface requires an `ExecuteAsync` method where you define the task to be performed.

Here is a simple job definition:

```csharp
public class MyJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing MyJob...");
        return Task.CompletedTask;
    }
}
```

In this example, `MyJob` simply writes "Executing MyJob..." to the console each time it is executed.

### Registering the Job in a Module

After creating a job, you need to register it within your module to make it part of your application's job infrastructure. This is done using the `AddJob<TJob>()` method in your module configuration:

```csharp
public override Task OnConfigureAsync(ModularityContext context)
{
    context.AddJob<MyJob>();
    return base.OnConfigureAsync(context);
}
```

This ensures that `MyJob` is added to the application’s job registry and ready to be executed as required.

### Background Job vs. Cron Job

Bonyan allows you to define both **background jobs** and **cron jobs**:

- **Background Job**: A job that is executed by the application when triggered, without a specific schedule. This type of job is useful for ad-hoc or on-demand processes.
- **Cron Job**: A job that runs on a specified schedule, defined using a cron expression. Cron jobs are ideal for automating repetitive tasks, such as sending email notifications or clearing logs periodically.

To define a cron job, use the `[CronJob]` attribute with a cron expression. Here is an example:

```csharp
[CronJob("*/1 * * * *")]
public class ScheduledJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Scheduled job executed: Tick Tok");
        return Task.CompletedTask;
    }
}
```

In this example, the `[CronJob("*/1 * * * *")]` attribute is used to define a schedule where the job runs every minute. The cron expression can be customized to match your application's scheduling requirements.

## Job Examples

### Basic Job Example

Below is an example of a basic job that implements the `IJob` interface. This job writes a message to the console each time it runs.

```csharp
public class MyJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing MyJob...");
        return Task.CompletedTask;
    }
}
```

This basic job runs whenever it is triggered by the application context. It simply logs a message to indicate its execution.

### Cron Job Example

If you need to run a job on a schedule, you can use the `[CronJob]` attribute. Below is an example of a cron job that runs every minute:

```csharp
[CronJob("*/1 * * * *")]
public class ScheduledJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Scheduled job executed: Tick Tok");
        return Task.CompletedTask;
    }
}
```

The cron expression `"*/1 * * * *"` specifies that the job runs every minute. You can customize the cron schedule to match your application's requirements.

## Summary

- **Add Job Module Dependency**: Ensure your module depends on `BonyanJobModule` to use job features.
- **Create a Job**: Implement the `IJob` interface and define the `ExecuteAsync` method to specify the task.
- **Register the Job**: Add the job to your module using `context.AddJob<TJob>()` to make it part of the application infrastructure.
- **Background vs. Cron Jobs**: Background jobs execute without a specific schedule, while cron jobs run based on a defined schedule using the `[CronJob]` attribute.

These features allow you to manage background operations efficiently, enhancing the scalability and responsiveness of your application.

