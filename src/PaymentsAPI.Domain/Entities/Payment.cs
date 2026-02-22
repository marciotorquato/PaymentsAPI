using PaymentsAPI.Domain.Enums;

namespace PaymentsAPI.Domain.Entities;

public class Payment
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public decimal Total { get; set; }
    public StatusPagamento Status { get; set; } = StatusPagamento.Pendente;
    public DateTimeOffset DataCriacao { get; set; }


    public virtual ICollection<PaymentItem> Items { get; set; } = [];
}