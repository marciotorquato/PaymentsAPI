namespace PaymentsAPI.Domain.Entities;

public class PaymentItem
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public Guid GameId { get; set; }
    public decimal Preco { get; set; }

    // Navigation Property
    public virtual Payment Payment { get; set; }
}
