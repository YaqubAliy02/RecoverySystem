using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecoverySystem.BuildingBlocks.Messaging.Configuration;
using RecoverySystem.BuildingBlocks.Messaging.RabbitMQ;

namespace RecoverySystem.BuildingBlocks.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));

        services.AddSingleton<IRabbitMQPublisher, RabbitMQPublisher>();
        services.AddSingleton<IRabbitMQConsumer, RabbitMQConsumer>();

        return services;
    }
}