using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Data;
using PaymentsAPI.IoC;
using PaymentsAPI.Messaging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthenticationConfig(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.AddSerilogConfiguration();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PaymentDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("MS_PaymentAPI")));
builder.Services.AddInfrastructure(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

try
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<RabbitMQInitializer>();
    await initializer.InitializeAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Erro ao inicializar RabbitMQ");
    throw;
}

try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    db.Database.Migrate();
    Log.Information("Migrations aplicadas com sucesso.");
}
catch(Exception ex)
{
    Log.Error($"Migrations Não foram aplicadas. {ex.Message}");
    throw;
}

// app.UseHttpsRedirection(); ← removido, não funciona em container sem HTTPS
app.UseAuthorization();
app.MapControllers();
app.Run();