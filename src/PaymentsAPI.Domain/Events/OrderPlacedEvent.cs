namespace PaymentsAPI.Domain.Events;

public class OrderPlacedEvent
{
    public Guid UsuarioId { get; set; }
    public Guid GameId { get; set; }
    public decimal PrecoAquisicao { get; set; }
    public DateTimeOffset DataCompra { get; set; }
}