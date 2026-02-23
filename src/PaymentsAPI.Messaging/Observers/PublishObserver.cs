using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentsAPI.Messaging.Observers;

public class PublishObserver : IPublishObserver
{
    private readonly ILogger<PublishObserver> _logger;

    public PublishObserver(ILogger<PublishObserver> logger)
    {
        _logger = logger;
    }

    public Task PrePublish<T>(PublishContext<T> context) where T : class
    {
        _logger.LogInformation(
            "🟡 PRE-PUBLISH | MessageType: {MessageType} | DestinationAddress: {Destination}",
            typeof(T).Name,
            context.DestinationAddress);

        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        _logger.LogInformation(
            "✅ POST-PUBLISH | MessageType: {MessageType}",
            typeof(T).Name);

        return Task.CompletedTask;
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
    {
        _logger.LogError(exception,
            "❌ PUBLISH-FAULT | MessageType: {MessageType}",
            typeof(T).Name);

        return Task.CompletedTask;
    }
}