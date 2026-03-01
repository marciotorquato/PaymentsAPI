using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PaymentsAPI.Application.Consumers;
using PaymentsAPI.Data;
using PaymentsAPI.Domain.Interfaces;
using PaymentsAPI.Messaging;

namespace PaymentsAPI.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("MS_PaymentAPI"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sqlOptions.MigrationsAssembly(typeof(PaymentDbContext).Assembly.FullName);
                });
        });

        // Registrar RabbitMQ Initializer
        services.AddSingleton<RabbitMQInitializer>();

        // Registrar IEventPublisher
        services.AddSingleton<IEventPublisher, RabbitMQEventPublisher>();

        // Registrar Consumer
        services.AddScoped<OrderPlacedConsumer>();

        // Registrar Background Service
        services.AddHostedService<RabbitMQConsumer>();

        return services;
    }
}