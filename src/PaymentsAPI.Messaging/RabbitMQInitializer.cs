using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace PaymentsAPI.Messaging;

public class RabbitMQInitializer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQInitializer> _logger;

    public RabbitMQInitializer(IConfiguration configuration, ILogger<RabbitMQInitializer> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            UserName = _configuration["RabbitMQ:Username"] ?? "admin",
            Password = _configuration["RabbitMQ:Password"] ?? "admin",
            VirtualHost = "/",
            Port = 5672
        };

        try
        {
            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            // Criar Exchange para PaymentProcessedEvent
            var exchangeName = _configuration["RabbitMQ:Exchanges:PaymentProcessed"] ?? "payment-processed-exchange";
            await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Fanout,
                durable: true,
                autoDelete: false,
                arguments: null
            );

            // Criar Queue para PaymentProcessedEvent
            var queueName = "payment-processed-queue";
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            // Vincular Queue à Exchange
            await channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "",
                arguments: null
            );

            _logger.LogInformation(
                "RabbitMQ inicializado com sucesso | Exchange: {Exchange} | Queue: {Queue}",
                exchangeName,
                queueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar RabbitMQ");
            throw;
        }
    }
}