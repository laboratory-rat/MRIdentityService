using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRPackage.Consts;
using MRPackage.Model;
using MRPackage.Options;
using RabbitMQ.Client;
using System;

namespace MRPackage.Rabbit
{
    public class RabbitMessageSender<TModel> : IDisposable
        where TModel : class, new()
    {
        protected readonly RabbitOptions _options;
        protected readonly ILogger _logger;
        protected readonly RabbitEnv _rabbitEnv;
        protected readonly RabbitServices _rabbitServices;
        protected readonly ConnectionFactory _connectionFactory;

        public RabbitMessageSender(RabbitEnv rabbitEnv, RabbitServices rabbitServices, IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            _rabbitEnv = rabbitEnv;
            _rabbitServices = rabbitServices;
            _logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(GetType());
            _options = provider.GetRequiredService<IOptions<RabbitOptions>>()?.Value ?? throw new ArgumentNullException(nameof(RabbitOptions));
            _connectionFactory = new ConnectionFactory { HostName = _options.Host };
        }

        public void Push(TModel model)
        {
            using (var connection = _connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare($"{_rabbitEnv.ToString()}.{_rabbitServices.ToString()}", exclusive: false);
                var body = MRRabbitMessageModel.Serialize(model, _rabbitServices.ToString(), _rabbitEnv.ToString());
                channel.BasicPublish(exchange: "", routingKey: $"{_rabbitEnv.ToString()}.{_rabbitServices.ToString()}", basicProperties: null, body: body);
            }
        }

        public static TSender Factory<TSender>(RabbitEnv rabbitEnv, RabbitServices rabbitService, IServiceCollection services)
            where TSender : RabbitMessageSender<TModel>
            => (TSender) Activator.CreateInstance(typeof(TSender), rabbitEnv, rabbitService, services);

        public void Dispose() { }
    }
}
