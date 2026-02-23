using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentsAPI.Domain.Events;
using PaymentsAPI.Domain.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace PaymentsAPI.Messaging;

public class RabbitMQEventPublisher : IEventPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQEventPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMQEventPublisher(
        IConfiguration configuration,
        ILogger<RabbitMQEventPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Criar conexão e canal do RabbitMQ
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:Username"] ?? "admin",
            Password = _configuration["RabbitMQ:Password"] ?? "admin",
            VirtualHost = "/",
            Port = 5672
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
    }

    public async Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentEvent)
    {
        try
        {
            var exchangeName = _configuration["RabbitMQ:Exchanges:PaymentProcessed"] ?? "payment-processed-exchange";

            // Serializar o evento para JSON
            var message = JsonSerializer.Serialize(paymentEvent);
            var body = Encoding.UTF8.GetBytes(message);

            // Publicar na exchange
            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: "",
                mandatory: false,
                basicProperties: new BasicProperties
                {
                    ContentType = "application/json",
                    DeliveryMode = DeliveryModes.Persistent,
                    Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                },
                body: body
            );

            _logger.LogInformation(
                "Evento PaymentProcessed publicado | UsuarioId: {UsuarioId} | GameId: {GameId} | Status: {Status}",
                paymentEvent.UsuarioId,
                paymentEvent.GameId,
                paymentEvent.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar PaymentProcessed");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}