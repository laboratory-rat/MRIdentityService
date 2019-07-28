using Api.Services;
using BLL.MessageSender;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MRPackage.Consts;
using MRPackage.Message;
using MRPackage.Rabbit;

namespace Api.Init
{
    public static class Services
    {
        public static void InitServices(this IServiceCollection services, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            services.AddSingleton<IHostedService, EmailReceiverService>(provider => 
                RabbitBackgroundReceiver<EmailMessage>.Factory<EmailReceiverService>(
                    RabbitServices.EMAIL, 
                    env.IsDevelopment() ? RabbitEnv.DEV : RabbitEnv.PROD, 
                    services)
                );

            services.AddTransient<EmailMessageSender>(provider =>
                RabbitMessageSender<EmailMessage>.Factory<EmailMessageSender>(RabbitEnv.DEV, RabbitServices.EMAIL, services));
        }
    }


}
