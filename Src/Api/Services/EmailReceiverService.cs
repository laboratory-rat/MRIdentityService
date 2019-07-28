using EmailService;
using Microsoft.Extensions.DependencyInjection;
using MRPackage.Consts;
using MRPackage.Message;
using MRPackage.Model;
using MRPackage.Rabbit;
using System.Threading.Tasks;

namespace Api.Services
{
    public class EmailReceiverService : RabbitBackgroundReceiver<EmailMessage>
    {
        protected readonly EmailSender _emailSender;

        public EmailReceiverService(RabbitServices rabbitService, RabbitEnv rabbitEnv, IServiceCollection collection) : base(rabbitService, rabbitEnv, collection)
        {
            _emailSender = collection.BuildServiceProvider().GetRequiredService<EmailSender>();
        }

        protected override async Task Received(MRRabbitMessageModel model, EmailMessage message)
        {
            await _emailSender.Send(message);
        }
    }
}
