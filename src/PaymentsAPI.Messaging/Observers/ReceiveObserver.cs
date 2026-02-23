using MassTransit;
using Microsoft.Extensions.Logging;

namespace PaymentsAPI.Messaging.Observers;

public class ReceiveObserver : IReceiveObserver
{
    private readonly ILogger<ReceiveObserver> _logger;

    public ReceiveObserver(ILogger<ReceiveObserver> logger)
    {
        _logger = logger;
    }

    public Task PreReceive(ReceiveContext context)
    {
        _logger.LogInformation(
            "🟢 PRE-RECEIVE (Mensagem Chegou no Endpoint) | InputAddress: {InputAddress} | ContentType: {ContentType}",
            context.InputAddress,
            context.ContentType);

        return Task.CompletedTask;
    }

    public Task PostReceive(ReceiveContext context)
    {
        _logger.LogInformation(
            "✅ POST-RECEIVE (Mensagem Processada pelo Endpoint) | InputAddress: {InputAddress}",
            context.InputAddress);

        return Task.CompletedTask;
    }

    public Task PostConsume<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType) where T : class
    {
        _logger.LogInformation(
            "✅ POST-CONSUME (Consumer Executou) | Consumer: {Consumer} | MessageType: {MessageType} | Duration: {Duration}ms | MessageId: {MessageId}",
            consumerType,
            typeof(T).Name,
            duration.TotalMilliseconds,
            context.MessageId);

        return Task.CompletedTask;
    }

    public Task ConsumeFault<T>(ConsumeContext<T> context, TimeSpan duration, string consumerType, Exception exception) where T : class
    {
        _logger.LogError(exception,
            "❌ CONSUME-FAULT (Erro no Consumer) | Consumer: {Consumer} | MessageType: {MessageType} | Duration: {Duration}ms | MessageId: {MessageId}",
            consumerType,
            typeof(T).Name,
            duration.TotalMilliseconds,
            context.MessageId);

        return Task.CompletedTask;
    }

    public Task ReceiveFault(ReceiveContext context, Exception exception)
    {
        _logger.LogError(exception,
            "❌ RECEIVE-FAULT (Erro ao Receber Mensagem) | InputAddress: {InputAddress}",
            context.InputAddress);

        return Task.CompletedTask;
    }
}