using Microsoft.EntityFrameworkCore;
using PaymentsAPI.Data.Repositories;
using PaymentsAPI.IoC;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthenticationConfig(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddControllers();
builder.AddSerilogConfiguration();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PaymentDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("MS_PaymentAPI")));
//builder.Services.AddApplicationServices();
//builder.Services.AddDomainServices();
//builder.Services.AddRepositories();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
