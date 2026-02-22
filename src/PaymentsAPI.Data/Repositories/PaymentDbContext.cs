using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Data.Configurations;
using PaymentsAPI.Domain.Entities;

namespace PaymentsAPI.Data.Repositories;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentItem> PaymentItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar todas as configurações
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentItemConfiguration());

        // Ou aplicar todas de uma vez:
        // modelBuilder.ApplyConfigurationsFromAssembly(typeof(PaymentConfiguration).Assembly);
    }
}