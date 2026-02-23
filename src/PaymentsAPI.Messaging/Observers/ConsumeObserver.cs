using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentsAPI.Messaging.Observers;

public class ConsumeObserver : IConsumeObserver
{
    private readonly ILogger<ConsumeObserver> _logger;

    public ConsumeObserver(ILogger<ConsumeObserver> logger)
    {
        _logger = logger;
    }

    public Task PreConsume<T>(ConsumeContext<T> context) where T : class
    {
        _logger.LogInformation(
            "🔵 PRE-CONSUME | MessageType: {MessageType} | MessageId: {MessageId} | ConversationId: {ConversationId}",
            typeof(T).Name,
            context.MessageId,
            context.ConversationId);

        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context) where T : class
    {
        _logger.LogInformation(
            "✅ POST-CONSUME (Sucesso) | MessageType: {MessageType} | MessageId: {MessageId}",
            typeof(T).Name,
            context.MessageId);

        return Task.CompletedTask;
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
    {
        _logger.LogError(exception,
            "❌ CONSUME-FAULT (Erro no Consumer) | MessageType: {MessageType} | MessageId: {MessageId}",
            typeof(T).Name,
            context.MessageId);

        return Task.CompletedTask;
    }
}