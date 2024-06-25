using ItemService.EventProcessor;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ItemService.RabbitMqClient
{
    public class RabbitMqSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly string _nomeDaFila;
        private readonly IConnection _connection;
        private IModel _channel;
        private IProcessaEvento _processoEvento;

        public RabbitMqSubscriber(IConfiguration configuration, IProcessaEvento processoEvento)
        {
            _configuration = configuration;
            _connection = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = Int32.Parse(_configuration["RabbitMQPort"])
            }.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _nomeDaFila = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _nomeDaFila, exchange: "trigger", routingKey: "");
            _processoEvento = processoEvento;

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            EventingBasicConsumer? consumidor = new EventingBasicConsumer(_channel);

            consumidor.Received += (ModuleHandle, eventArgs) =>
            {
                ReadOnlyMemory<byte> body = eventArgs.Body;
                var mensagem = Encoding.UTF8.GetString(body.ToArray());
                _processoEvento.Processo(mensagem);
            };

            _channel.BasicConsume(queue: _nomeDaFila, autoAck: true, consumer: consumidor);
            return Task.CompletedTask;
        }
    }
}
