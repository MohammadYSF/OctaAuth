namespace Auth.RabbitMqBus;
public static class ServiceExtentions
{
    public static WebApplicationBuilder ConfigureBus(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection(nameof(RabbitMqConfig)));
        builder.Services.AddSingleton<IEventBus, RabbitMQBus>();
        return builder;

    }
}
