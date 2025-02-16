using DeepIn.Messaging.API.Domain;
using DeepIn.Messaging.API.Infrastructure.Repositories;
using DeepIn.Messaging.API.Services;
using DeepIn.Messaging.API;
using MongoDB.Driver;
using Deepin.EventBus.RabbitMQ;
using Deepin.ServiceDefaults.Extensions;
using Deepin.Infrastructure.Caching;

namespace Deepin.Messaging.API.Extensions;

public static class HostExtensions
{
    public static WebApplicationBuilder AddApplicationService(this WebApplicationBuilder builder)
    {

        builder.AddServiceDefaults();
        builder.Services
        .AddMessagingContext(builder.Configuration.GetRequiredConnectionString("DefaultConnection"))
        .AddEventBusRabbitMQ(
            builder.Configuration.GetSection(RabbitMqOptions.ConfigurationKey).Get<RabbitMqOptions>() ?? throw new ArgumentNullException("RabbitMQ"),
            assembly: typeof(HostExtensions).Assembly)
        .AddDefaultCache(new RedisCacheOptions
        {
            ConnectionString = builder.Configuration.GetConnectionString("Redis"),
        })
        .AddDefaultUserContexts();

        builder.Services.AddScoped<IMessageService, MessageService>();
        return builder;
    }
    public static WebApplication UseApplicationService(this WebApplication app)
    {
        app.UseServiceDefaults();

        return app;
    }
    private static IServiceCollection AddMessagingContext(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton(s => new MongoClient(connectionString));
        services.AddSingleton(s => new MessagingContext(s.GetRequiredService<MongoClient>(), "messages"));
        services.AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        return services;
    }
}