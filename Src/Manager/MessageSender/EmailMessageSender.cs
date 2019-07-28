using Microsoft.Extensions.DependencyInjection;
using MRPackage.Consts;
using MRPackage.Message;
using MRPackage.Rabbit;

namespace BLL.MessageSender
{
    public class EmailMessageSender : RabbitMessageSender<EmailMessage>
    {
        public EmailMessageSender(RabbitEnv rabbitEnv, RabbitServices rabbitServices, IServiceCollection services) : base(rabbitEnv, rabbitServices, services)
        {
        }
    }
}
