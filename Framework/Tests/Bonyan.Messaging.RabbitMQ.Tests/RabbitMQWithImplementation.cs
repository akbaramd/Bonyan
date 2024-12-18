﻿using Bonyan.Modularity;
using Bonyan.Modularity.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Bonyan.Messaging.RabbitMQ.Tests;

public class RabbitMqWithImplementation : BonModule
{

    public RabbitMqWithImplementation()
    {
      
    }
    public override Task OnConfigureAsync(BonConfigurationContext context)
    {
        context.AddMessaging(c =>
        {
            c.RegisterConsumer<TestEventConsumer>();
            c.AddRabbitMq(options =>
            {
                options.HostName = "localhost";
                options.Port = 5672;
                options.UserName = "guest";
                options.Password = "guest";
                options.VirtualHost = "/";
                
                options.ConfigureConsumer<TestEventConsumer>("test");
            });
        });
        

        return Task.CompletedTask;
    }
}