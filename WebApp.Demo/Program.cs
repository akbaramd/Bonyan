using Microsoft.EntityFrameworkCore;
using Novin.AspNetCore.Novin.AspNetCore;
using Novin.AspNetCore.Novin.AspNetCore.EntityFrameworkCore;
using Novin.AspNetCore.Novin.AspNetCore.Extensions;
using Novin.AspNetCore.Novin.AspNetCore.Mediator;
using Novin.AspNetCore.Novin.AspNetCore.Messaging;
using WebApp.Demo.Data;
using WebApp.Demo.Events;

var application = await NovinApplication.CreateBuilder("web-api", ctx =>
    {
        ctx.AddEndpoints()
            .AddMediator(c => { c.UseMessagingForDomainEvent(); })
         
            .AddMessaging(c =>
            {
                c.RegisterConsumer<BookCreatedEventConsumer>();
                
                c.AddRabbitMq(options =>
                {
                    options.HostName = "localhost";
                    options.Port = 5672;
                    options.UserName = "guest";
                    options.Password = "guest";
                    options.VirtualHost = "/";

                    options.ConfigureConsumer<BookCreatedEventConsumer>();
                });

                c.AddOutbox(outBoxConfiguration => { outBoxConfiguration.UseEntityFrameworkCoreStore<AppDBContext>(); });
            });
    })
    .BuildAsync(ctx =>
    {
        ctx.UseMediatorEndpoints(c => { c.MapEvent<TestEvent>("/mediator/test"); });

        ctx.UseMessagingEndpoints(c => { c.MapSend<TestEvent, TestEventResponse>("/message/test"); });
    });

await application.RunAsync();