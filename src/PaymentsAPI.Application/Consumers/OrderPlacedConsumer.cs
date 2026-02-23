using Microsoft.Extensions.Logging;
using PaymentsAPI.Data;
using PaymentsAPI.Domain.Entities;
using PaymentsAPI.Domain.Enums;
using PaymentsAPI.Domain.Events;
using PaymentsAPI.Domain.Interfaces;

namespace PaymentsAPI.Application.Consumers;

public class OrderPlacedConsumer
{
    private readonly ILogger<OrderPlacedConsumer> _logger;
    private readonly PaymentDbContext _context;
    private readonly IEventPublisher _eventPublisher;

    public OrderPlacedConsumer(
        ILogger<OrderPlacedConsumer> logger,
        PaymentDbContext context,
        IEventPublisher eventPublisher)
    {
        _logger = logger;
        _context = context;
        _eventPublisher = eventPublisher;
    }

    public async Task ProcessAsync(OrderPlacedEvent orderEvent)
    {
        _logger.LogInformation("🎯 CONSUMER EXECUTADO | UsuarioId: {UsuarioId} | GameId: {GameId}",
            orderEvent.UsuarioId, orderEvent.GameId);

        try
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                UsuarioId = orderEvent.UsuarioId,
                Total = orderEvent.PrecoAquisicao,
                Status = StatusPagamento.Aprovado,
                DataCriacao = DateTimeOffset.UtcNow
            };

            var paymentItem = new PaymentItem
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                GameId = orderEvent.GameId,
                Preco = orderEvent.PrecoAquisicao
            };

            payment.Items.Add(paymentItem);

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Pagamento criado | PaymentId: {PaymentId}", payment.Id);

            var paymentProcessedEvent = new PaymentProcessedEvent
            {
                UsuarioId = orderEvent.UsuarioId,
                GameId = orderEvent.GameId,
                Status = StatusPagamento.Aprovado.ToString(),
                DataProcessamento = DateTimeOffset.UtcNow
            };

            await _eventPublisher.PublishPaymentProcessedAsync(paymentProcessedEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao processar pedido");
            throw;
        }
    }
}