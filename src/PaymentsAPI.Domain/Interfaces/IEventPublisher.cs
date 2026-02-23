using PaymentsAPI.Domain.Events;

namespace PaymentsAPI.Domain.Interfaces;
public interface IEventPublisher
{
    Task PublishPaymentProcessedAsync(PaymentProcessedEvent paymentEvent);
}