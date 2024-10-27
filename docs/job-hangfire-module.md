# Hangfire Job Module Guide

The **Bonyan Job Module with Hangfire** significantly extends the capabilities of the Bonyan Job Module, allowing developers to manage background jobs using the **Hangfire** library. This integration facilitates advanced job management features, such as persistence, retries, and monitoring, providing a sophisticated solution for effectively managing background job execution within modular applications.

> **Note**: If you are unfamiliar with the fundamental concepts of jobs in Bonyan, please refer to the [Job Module Guide](job_module_guide.md) first for a foundational understanding.

## Table of Contents
- [Installation](#installation)
- [Step-by-Step Integration with Hangfire](#step-by-step-integration-with-hangfire)
  - [Adding the Hangfire Job Module Dependency](#adding-the-hangfire-job-module-dependency)
  - [Creating a Hangfire-Compatible Job](#creating-a-hangfire-compatible-job)
  - [Registering the Job with Hangfire](#registering-the-job-with-hangfire)
  - [Handling Job Scheduling with Hangfire](#handling-job-scheduling-with-hangfire)
- [Job Examples](#job-examples)
  - [Basic Hangfire Job Example](#basic-hangfire-job-example)
  - [Cron Hangfire Job Example](#cron-hangfire-job-example)
- [Summary](#summary)

## Installation

To install the Hangfire Job Module, run the following command in your terminal to add the necessary support for managing jobs with Hangfire:

```bash
dotnet add package Bonyan.AspNetCore.Job.Hangfire
```

This command will include all required libraries and tools to integrate Hangfire, providing enhanced job management capabilities within your application modules.

Once the module is set up, you can access the Hangfire dashboard at `/hangfire` to monitor and manage your jobs effectively. This dashboard provides insights into job execution, current status, retries, and scheduling, allowing full control over background job operations.

## Step-by-Step Integration with Hangfire

The following steps will guide you through the process of integrating Hangfire into your Bonyan-based application to manage background jobs efficiently.

### Adding the Hangfire Job Module Dependency

To leverage the **Hangfire Job Module**, your main module must declare a dependency on `BonyanJobHangfireModule`. This ensures that Hangfire's capabilities, including job persistence, retries, and monitoring, are available for managing jobs effectively.

Here is how to declare the dependency in your module:

```csharp
[DependOn(typeof(BonyanJobHangfireModule))]
public class MyMainModule : Module
{
    public override Task OnConfigureAsync(ModularityContext context)
    {
        // Adding a Hangfire job to the context
        context.AddJob<MyJob>();
        return base.OnConfigureAsync(context);
    }
}
```

In this example, `MyMainModule` depends on `BonyanJobHangfireModule`, which allows it to integrate Hangfire's job management features. Adding jobs is done using `context.AddJob<TJob>()`, enabling seamless job handling.

### Creating a Hangfire-Compatible Job

Jobs in the Bonyan framework are defined by implementing the `IJob` interface. The `ExecuteAsync` method specifies the task to be performed. This approach remains consistent whether you are integrating basic jobs or Hangfire-compatible jobs.

Below is an example of a simple job that works seamlessly with Hangfire:

```csharp
public class MyJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing MyJob with Hangfire...");
        return Task.CompletedTask;
    }
}
```

In this job, `MyJob` logs a message ("Executing MyJob with Hangfire...") to the console each time it is executed. Hangfire handles scheduling, retries, and persistence for the job.

### Registering the Job with Hangfire

After creating a job, you need to register it in your module to ensure Hangfire can manage its lifecycle, including scheduling, persistence, and retries.

```csharp
public override Task OnConfigureAsync(ModularityContext context)
{
    context.AddJob<MyJob>();
    return base.OnConfigureAsync(context);
}
```

This configuration ensures that `MyJob` is registered with Hangfire, making it part of the application’s background job registry and ready to be managed through the Hangfire dashboard.

### Handling Job Scheduling with Hangfire

Hangfire allows jobs to be scheduled using cron expressions or other triggers, which provides flexibility in job execution. By applying the `[CronJob]` attribute, you can set up jobs to run at specific intervals.

Here is an example of how to define a cron job with Hangfire:

```csharp
[CronJob("0 0 * * *")] // This cron expression schedules the job to run daily at midnight
public class DailyJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing DailyJob with Hangfire...");
        return Task.CompletedTask;
    }
}
```

In this example, `DailyJob` is configured to run every day at midnight. Hangfire manages this job, including execution retries and logging.

## Job Examples

### Basic Hangfire Job Example

Here is a straightforward example of a basic job that works with Hangfire. This job logs a message to the console each time it runs.

```csharp
public class MyJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing MyJob with Hangfire...");
        return Task.CompletedTask;
    }
}
```

This job will be executed whenever it is triggered by Hangfire, and it will log the message "Executing MyJob with Hangfire..." to indicate successful execution.

### Cron Hangfire Job Example

If you need to run a job on a specific schedule, you can use the `[CronJob]` attribute. Below is an example of a job that runs daily:

```csharp
[CronJob("0 0 * * *")] // Runs every day at midnight
public class DailyJob : IJob
{
    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Executing DailyJob with Hangfire...");
        return Task.CompletedTask;
    }
}
```

The cron expression `"0 0 * * *"` configures the job to execute at midnight every day. Hangfire will ensure that the job is run according to this schedule, handling retries in case of failure.

## Summary

- **Add Hangfire Job Module Dependency**: Ensure your module depends on `BonyanJobHangfireModule` to utilize Hangfire for job management.
- **Create a Job**: Implement the `IJob` interface and define the `ExecuteAsync` method to specify the task.
- **Register the Job**: Add the job to your module using `context.AddJob<TJob>()` to make it part of the Hangfire infrastructure.
- **Schedule Jobs with Hangfire**: Use the `[CronJob]` attribute to specify job schedules for automating recurring tasks.

By using **Bonyan.Job with Hangfire**, you benefit from advanced background processing capabilities, including support for retries, persistence, and monitoring. This greatly enhances the reliability, maintainability, and scalability of your applications.

