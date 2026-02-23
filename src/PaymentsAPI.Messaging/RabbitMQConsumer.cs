using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PaymentsAPI.Application.Consumers;
using PaymentsAPI.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace PaymentsAPI.Messaging;

public class RabbitMQConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMQConsumer(
        IConfiguration configuration,
        ILogger<RabbitMQConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
                UserName = _configuration["RabbitMQ:Username"] ?? "admin",
                Password = _configuration["RabbitMQ:Password"] ?? "admin",
                VirtualHost = "/",
                Port = 5672
            };

            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            var queueName = "order-placed-queue";

            // Configurar QoS
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);

            // Declarar consumer assíncrono
            var consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation("🟢 Mensagem Recebida | Queue: {Queue} | Body: {Message}", queueName, message);

                    // Deserializar JSON
                    var orderEvent = JsonSerializer.Deserialize<OrderPlacedEvent>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (orderEvent != null)
                    {
                        // Processar mensagem usando o Consumer
                        using var scope = _serviceProvider.CreateScope();
                        var orderConsumer = scope.ServiceProvider.GetRequiredService<OrderPlacedConsumer>();

                        await orderConsumer.ProcessAsync(orderEvent);

                        // ACK manual
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.LogInformation("✅ Mensagem Processada e ACK enviado");
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Falha ao deserializar mensagem");
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao processar mensagem");

                    // NACK com requeue
                    try
                    {
                        await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                    }
                    catch (Exception nackEx)
                    {
                        _logger.LogError(nackEx, "Erro ao enviar NACK");
                    }
                }
            };

            // Iniciar consumer
            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken
            );

            _logger.LogInformation("✅ RabbitMQ Consumer iniciado | Queue: {Queue}", queueName);

            // Manter o serviço rodando enquanto não for cancelado
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("RabbitMQ Consumer foi cancelado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro fatal no RabbitMQ Consumer");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando RabbitMQ Consumer...");

        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }

        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}