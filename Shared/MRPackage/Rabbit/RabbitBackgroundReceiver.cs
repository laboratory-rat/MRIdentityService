using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MRPackage.Consts;
using MRPackage.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using MRPackage.Model;
using Newtonsoft.Json;

namespace MRPackage.Rabbit
{
    public abstract class RabbitBackgroundReceiver<TMessageType> : BackgroundService
        where TMessageType : class, new()
    {
        protected readonly RabbitOptions _options;
        protected readonly ILogger _logger;
        protected readonly int MaxFail;
        protected readonly RabbitServices _rabbitService;
        protected readonly RabbitEnv _rabbitEnv;
        protected IConnection _connection;
        protected IModel _channel;

        public RabbitBackgroundReceiver(RabbitServices rabbitService, RabbitEnv rabbitEnv, IServiceCollection collection)
        {
            var provider = collection.BuildServiceProvider();

            _rabbitService = rabbitService;
            _rabbitEnv = rabbitEnv;
            _logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
            _options = provider.GetRequiredService<IOptions<RabbitOptions>>().Value ?? throw new ArgumentNullException(nameof(RabbitOptions));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            InitRabbit();
        }

        protected virtual void InitRabbit()
        {
            var factory = new ConnectionFactory { HostName = _options.Host };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (Rmodel, ea) =>
            {
                var (message, model) = MRRabbitMessageModel.Parse<TMessageType>(ea.Body);
                await Received(message, model);
            };

            _channel.BasicConsume($"{_rabbitEnv.ToString()}.{_rabbitService.ToString()}", autoAck: true, consumer);
        }

        protected abstract Task Received(MRRabbitMessageModel model, TMessageType message);

        public override void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
            base.Dispose();
        }

        public static TService Factory<TService>(RabbitServices rabbitService, RabbitEnv rabbitEnv, IServiceCollection services)
            where TService : RabbitBackgroundReceiver<TMessageType>
            => (TService)Activator.CreateInstance(typeof(TService), rabbitService, rabbitEnv, services);
    }
}
