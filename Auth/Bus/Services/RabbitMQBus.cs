using Auth.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace Auth.RabbitMqBus;
public sealed class RabbitMQBus : IEventBus
{

    private readonly Dictionary<string, List<Type>> _handlers;

    private readonly List<Type> _eventTypes;

    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly RabbitMqConfig _rabbitMqConfig;

    public RabbitMQBus(IServiceScopeFactory serviceScopeFactory, IOptions<RabbitMqConfig> rabbitMqConig)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _handlers = new Dictionary<string, List<Type>>();
        _eventTypes = new List<Type>();
        _rabbitMqConfig = rabbitMqConig.Value;
    }


    public void Publish<T>(T @event) where T : DomainEvent
    {
        ConnectionFactory connectionFactory = new ConnectionFactory();
        connectionFactory.HostName = _rabbitMqConfig.HostName;
        connectionFactory.UserName = _rabbitMqConfig.UserName;
        connectionFactory.Password = _rabbitMqConfig.Password;
        connectionFactory.Port = _rabbitMqConfig.Port;

        using IConnection connection = connectionFactory.CreateConnection();
        using IModel channel = connection.CreateModel();
        IBasicProperties basicProperties = channel.CreateBasicProperties();
        basicProperties.Persistent = true;
        string name = @event.GetType().Name;
        channel.QueueDeclare(name, durable: true, exclusive: false, autoDelete: false, null);
        string s = JsonConvert.SerializeObject(@event, Formatting.Indented);
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        channel.BasicPublish("", name, mandatory: true, basicProperties, bytes);
    }

    public void Subscribe<T, TH>() where T : DomainEvent where TH : IEventHandler<T>
    {
        string name = typeof(T).Name;
        Type handlerType = typeof(TH);
        if (!_eventTypes.Contains(typeof(T)))
        {
            _eventTypes.Add(typeof(T));
        }

        if (!_handlers.ContainsKey(name))
        {
            _handlers.Add(name, new List<Type>());
        }

        if (_handlers[name].Any((Type s) => s.GetType() == handlerType))
        {
            throw new ArgumentException($"Handler Type {handlerType.Name} already is registered for '{name}'", "handlerType");
        }

        _handlers[name].Add(handlerType);
        StartBasicConsume<T>();
    }

    private void StartBasicConsume<T>() where T : DomainEvent
    {
        ConnectionFactory connectionFactory = new ConnectionFactory();
        connectionFactory.HostName = _rabbitMqConfig.HostName;
        connectionFactory.UserName = _rabbitMqConfig.UserName;
        connectionFactory.Password = _rabbitMqConfig.Password;
        connectionFactory.Port = _rabbitMqConfig.Port;

        IConnection connection = connectionFactory.CreateConnection();
        IModel channel = connection.CreateModel();
        string name = typeof(T).Name;
        channel.QueueDeclare(name, durable: true, exclusive: false, autoDelete: false, null);
        var asyncEventingBasicConsumer = new EventingBasicConsumer(channel);
        asyncEventingBasicConsumer.Received += async (sender, e) =>
        {
            await Consumer_Received(sender, e);
        };
        channel.BasicConsume(name, autoAck: true, asyncEventingBasicConsumer);

    }

    private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
    {
        string eventName = e.RoutingKey;
        string message = Encoding.UTF8.GetString(e.Body.ToArray());
        try
        {
            await ProcessEvent(eventName, message).ConfigureAwait(continueOnCapturedContext: false);
        }
        catch (Exception)
        {
        }
    }

    private async Task ProcessEvent(string eventName, string message)
    {
        string eventName2 = eventName;
        if (!_handlers.ContainsKey(eventName2))
        {
            return;
        }

        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        List<Type> subscriptions = _handlers[eventName2];
        foreach (Type subscription in subscriptions)
        {
            object handler = scope.ServiceProvider.GetService(subscription);
            if (handler != null)
            {
                Type eventType = _eventTypes.SingleOrDefault((Type t) => t.Name == eventName2);
                object @event = JsonConvert.DeserializeObject(message, eventType);
                Type conreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                await (Task)conreteType.GetMethod("HandleAsync").Invoke(handler, new object[1] { @event });
            }
        }
    }
}