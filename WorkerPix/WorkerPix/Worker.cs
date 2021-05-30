using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WorkerPix.Models;

namespace WorkerPix
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly WorkerConfiguration _workerConfiguration;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _workerConfiguration = _configuration.BindTo("RABBITMQ", new WorkerConfiguration());
            _connectionFactory = _configuration.BindTo("RABBITMQ", new ConnectionFactory());
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel.QueueDeclare(queue: _workerConfiguration.Queue,
                                      durable: true,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

            _channel.QueueBind(_workerConfiguration.Queue,
                               _workerConfiguration.Exchange,
                               _workerConfiguration.RoutingKey,
                               null);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            _logger.LogInformation($"Queue [{_workerConfiguration.Queue}] is waiting for messages.");

            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (bc, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                try
                {
                    _logger.LogInformation($"Processing PIX: '{message}'.");
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (AlreadyClosedException)
                {
                    _logger.LogInformation("RabbitMQ is closed!");
                }
                catch (Exception e)
                {
                    _logger.LogError(default, e, e.Message);
                }
            };

            _channel.BasicConsume(queue: _workerConfiguration.Queue, autoAck: false, consumer: consumer);

            await Task.CompletedTask;

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection.Dispose();
            _logger.LogInformation("RabbitMQ connection is closed.");
        }
    }
}
