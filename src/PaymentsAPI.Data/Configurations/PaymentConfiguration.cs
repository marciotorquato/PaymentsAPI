using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentsAPI.Domain.Entities;
using PaymentsAPI.Domain.Enums;

namespace PaymentsAPI.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
               .ValueGeneratedNever();

        builder.Property(p => p.UsuarioId)
               .IsRequired();

        builder.Property(p => p.Total)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        builder.Property(p => p.Status)
               .HasColumnType("nvarchar(50)")
               .IsRequired()
               .HasMaxLength(50)
               .HasDefaultValue(StatusPagamento.Pendente);

        builder.Property(p => p.DataCriacao)
               .HasColumnType("datetimeoffset(7)")
               .IsRequired();

        // Relacionamento 1:N com PaymentItem
        builder.HasMany(p => p.Items)
               .WithOne(pi => pi.Payment)
               .HasForeignKey(pi => pi.PaymentId)
               .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(p => p.UsuarioId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.DataCriacao);
    }
}