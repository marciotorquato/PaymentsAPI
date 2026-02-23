using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentsAPI.Messaging.Observers;
public class SkippedMessageObserver : IConsumeMessageObserver<object>
{
    private readonly ILogger<SkippedMessageObserver> _logger;

    public SkippedMessageObserver(ILogger<SkippedMessageObserver> logger)
    {
        _logger = logger;
    }

    public Task PreConsume(ConsumeContext<object> context)
    {
        _logger.LogWarning(
            "⚠️ TENTANDO CONSUMIR MENSAGEM GENÉRICA | MessageId: {MessageId} | MessageType: {MessageType}",
            context.MessageId,
            context.Message?.GetType().Name ?? "Unknown");

        return Task.CompletedTask;
    }

    public Task PostConsume(ConsumeContext<object> context)
    {
        _logger.LogInformation(
            "✅ MENSAGEM GENÉRICA CONSUMIDA | MessageId: {MessageId}",
            context.MessageId);

        return Task.CompletedTask;
    }

    public Task ConsumeFault(ConsumeContext<object> context, Exception exception)
    {
        _logger.LogError(exception,
            "❌ ERRO AO CONSUMIR MENSAGEM GENÉRICA | MessageId: {MessageId}",
            context.MessageId);

        return Task.CompletedTask;
    }
}