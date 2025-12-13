using Bonyan.Mediators;
using Bonyan.Messaging.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Bonyan.Messaging;

/// <summary>
/// پیاده‌سازی پیش‌فرض IBonMessageBus که از سیستم مدیاتور استفاده می‌کند.
/// این کلاس به عنوان پیاده‌سازی پیش‌فرض عمل می‌کند و تمام عملیات را درون حافظه انجام می‌دهد.
/// زمانی که RabbitMQ یا پیاده‌سازی دیگری تنظیم نشده باشد، از این کلاس استفاده می‌شود.
/// </summary>
public class MediatorBonMessageBus : IBonMessageBus
{
    private readonly IBonMediator _mediator;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MediatorBonMessageBus> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public MediatorBonMessageBus(
        IBonMediator mediator,
        IServiceProvider serviceProvider,
        ILogger<MediatorBonMessageBus> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// ارسال درخواست به سرویس مشخص و انتظار برای پاسخ - از مدیاتور استفاده می‌کند.
    /// </summary>
    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        string destinationServiceName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class,IMessageRequest<TResponse>
        where TResponse : class
    {
        // Generate correlation ID if not provided
        if (string.IsNullOrEmpty(request.CorrelationId))
        {
            request.CorrelationId = Guid.NewGuid().ToString();
        }

        _logger.LogDebug("ارسال درخواست {RequestType} به سرویس {DestinationService} با شناسه همبستگی {CorrelationId}",
            typeof(TRequest).Name, destinationServiceName, request.CorrelationId);

        try
        {
            // تنظیم نام سرویس مقصد در درخواست
            request.TargetService = destinationServiceName;

                // بررسی اینکه آیا درخواست از قبل Command است
                if (request is IBonCommand<TResponse> command)
                {
                    _logger.LogDebug("استفاده از مدیاتور برای Command: {CommandType}", typeof(TRequest).Name);
                    return await _mediator.SendAsync(command, cancellationToken);
                }

            // اگر درخواست Command نیست، سعی می‌کنیم Command Handler مناسب پیدا کنیم
            var commandHandlerType = typeof(IBonCommandHandler<,>).MakeGenericType(typeof(TRequest), typeof(TResponse));
            var commandHandler = _serviceProvider.GetService(commandHandlerType);

            if (commandHandler != null)
            {
                _logger.LogDebug("پیدا کردن Command Handler برای: {RequestType}", typeof(TRequest).Name);
                var handleMethod = commandHandlerType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    var task = (Task<TResponse>)handleMethod.Invoke(commandHandler, new object[] { request, cancellationToken })!;
                    return await task;
                }
            }

            throw new InvalidOperationException(
                $"No Handler found for command type {typeof(TRequest).Name} with response type {typeof(TResponse).Name}. " +
                $"Please implement a handler that implements IBonCommandHandler<{typeof(TRequest).Name}, {typeof(TResponse).Name}>.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال درخواست {RequestType} به سرویس {DestinationService}",
                typeof(TRequest).Name, destinationServiceName);
            throw;
        }
    }

    /// <summary>
    /// ارسال درخواست به سرویس مشخص بدون انتظار برای پاسخ - از مدیاتور استفاده می‌کند.
    /// </summary>
    public async Task SendAsync<TRequest>(
        string destinationServiceName,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TRequest : class,IMessageRequest
    {
        // Generate correlation ID if not provided
        if (string.IsNullOrEmpty(request.CorrelationId))
        {
            request.CorrelationId = Guid.NewGuid().ToString();
        }

        _logger.LogDebug("ارسال درخواست بدون پاسخ {RequestType} به سرویس {DestinationService} با شناسه همبستگی {CorrelationId}",
            typeof(TRequest).Name, destinationServiceName, request.CorrelationId);

        try
        {
            // تنظیم نام سرویس مقصد در درخواست
            request.TargetService = destinationServiceName;

            // بررسی اینکه آیا درخواست از قبل Command است
            if (request is IBonCommand command)
            {
                _logger.LogDebug("استفاده از مدیاتور برای Command بدون پاسخ: {CommandType}", typeof(TRequest).Name);
                await _mediator.SendAsync(command, cancellationToken);
                return;
            }

            // اگر درخواست Command نیست، سعی می‌کنیم Command Handler مناسب پیدا کنیم
            var commandHandlerType = typeof(IBonCommandHandler<>).MakeGenericType(typeof(TRequest));
            var commandHandler = _serviceProvider.GetService(commandHandlerType);

            if (commandHandler != null)
            {
                _logger.LogDebug("پیدا کردن Command Handler برای: {RequestType}", typeof(TRequest).Name);
                var handleMethod = commandHandlerType.GetMethod("HandleAsync");
                if (handleMethod != null)
                {
                    var task = (Task)handleMethod.Invoke(commandHandler, new object[] { request, cancellationToken })!;
                    await task;
                    return;
                }
            }

            throw new InvalidOperationException(
                $"No Handler found for command type {typeof(TRequest).Name}. " +
                $"Please implement a handler that implements IBonCommandHandler<{typeof(TRequest).Name}>.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ارسال درخواست بدون پاسخ {RequestType} به سرویس {DestinationService}",
                typeof(TRequest).Name, destinationServiceName);
            throw;
        }
    }

    /// <summary>
    /// انتشار رویداد - از مدیاتور استفاده می‌کند.
    /// </summary>
    public async Task PublishAsync(
        IMessageEvent messageEvent,
        CancellationToken cancellationToken = default)
    {
        // Generate correlation ID if not provided
        if (string.IsNullOrEmpty(messageEvent.CorrelationId))
        {
            messageEvent.CorrelationId = Guid.NewGuid().ToString();
        }

        _logger.LogDebug("انتشار رویداد {EventType} با شناسه همبستگی {CorrelationId}",
            messageEvent.GetType().Name, messageEvent.CorrelationId);

        try
        {
            // بررسی اینکه آیا رویداد از قبل IBonEvent است
            _logger.LogDebug("استفاده از مدیاتور برای رویداد: {EventType}", messageEvent.GetType().Name);
            await _mediator.PublishAsync(messageEvent, cancellationToken);
            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در انتشار رویداد {EventType}", messageEvent.GetType().Name);
            throw;
        }
    }
}
