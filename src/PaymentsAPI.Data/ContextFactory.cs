using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PaymentsAPI.Data;

internal class ContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PaymentDbContext>();
        optionsBuilder.UseSqlServer("Data source=(localdb)\\mssqllocaldb;Initial Catalog=MS_PaymentAPI;Integrated security=true");

        return new PaymentDbContext(optionsBuilder.Options);
    }
}
