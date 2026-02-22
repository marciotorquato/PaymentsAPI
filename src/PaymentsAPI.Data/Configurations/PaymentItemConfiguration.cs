using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentsAPI.Domain.Entities;

namespace PaymentsAPI.Data.Configurations;

public class PaymentItemConfiguration : IEntityTypeConfiguration<PaymentItem>
{
    public void Configure(EntityTypeBuilder<PaymentItem> builder)
    {
        builder.ToTable("PaymentItem");

        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.Id)
               .ValueGeneratedNever();

        builder.Property(pi => pi.PaymentId)
               .IsRequired();

        builder.Property(pi => pi.GameId)
               .IsRequired();

        builder.Property(pi => pi.Preco)
               .HasColumnType("decimal(18,2)")
               .IsRequired();

        // Relacionamento N:1 com Payment
        builder.HasOne(pi => pi.Payment)
               .WithMany(p => p.Items)
               .HasForeignKey(pi => pi.PaymentId)
               .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(pi => pi.PaymentId);
        builder.HasIndex(pi => pi.GameId);
    }
}