using Amazon.Runtime;
using BLL;
using BLL.Connector.Bucket;
using DL;
using EmailService;
using EmailService.Options;
using Infrastructure.Entity.AppUser;
using Infrastructure.Interface.Manager;
using Infrastructure.Interface.Repository;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MRApiCommon.Extensions;
using MRApiCommon.Service;
using MRMongoTools.Extensions;
using MRPackage.Options;
using NLog;
using NLog.AWS.Logger;
using NLog.Config;
using TemplateService;
using TemplateService.Infrastructure.Interface;

namespace Api.Init
{
    public static class DIExtensions
    {
        public static IServiceCollection InitDI(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureMRToken(configuration, "TokenOptions");
            services.ConfigurateMRIdentity<User, RepositoryUser, ManagerUser>(configuration, "MongoDatabaseConnection", options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Tokens.AuthenticatorTokenProvider = JwtBearerDefaults.AuthenticationScheme;
            });

            services.Scan(scan =>
            {
                scan
                .FromAssemblyOf<IRepositoryProvider>()
                    .AddClasses()
                    .AsSelf()
                    .WithTransientLifetime()
                .FromAssemblyOf<RepositoryProvider>()
                    .AddClasses()
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .FromAssemblyOf<Manager>()
                    .AddClasses()
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
                .FromAssemblyOf<RepositoryRole>()
                    .AddClasses()
                    .AsSelf()
                    .WithTransientLifetime();
            });

            // connectors
            var bucketImageDOptions = new BucketOptions();
            configuration.Bind("DBucketOptions", bucketImageDOptions);
            services.AddTransient<ConnectorBucketImageD>(x => new ConnectorBucketImageD(Options.Create(bucketImageDOptions)));

            var bucketImageROptions = new BucketOptions();
            configuration.Bind("RBucketOptions", bucketImageROptions);
            services.AddTransient<ConnectorBucketImageR>(x => new ConnectorBucketImageR(Options.Create(bucketImageROptions)));

            services.Configure<EmailServiceOptions>(options => configuration.GetSection("EmailService").Bind(options));
            services.Configure<RabbitOptions>(options => configuration.GetSection("Rabbit").Bind(options));
            services.AddTransient<ITemplateBuilder, TemplateBuilder>();
            services.AddTransient<EmailSender>();

            // loggers
            var awsLogOptions = new AwsLogOptions();
            configuration.Bind("AwsLogOptions", awsLogOptions);
            var loggingConfig = new LoggingConfiguration();
            var awsTarget = new AWSTarget
            {
                Name = "aws",
                LogGroup = awsLogOptions.TargetGroup,
                Region = awsLogOptions.Region,
                Credentials = new BasicAWSCredentials(awsLogOptions.AccessKey, awsLogOptions.SecretKey),
                Layout = "[${longdate}] (${callsite}) ${newline} ${aspnet-mvc-controller}/${aspnet-mvc-action}/${aspnet-request} ${newline} ${aspnet-user-identity} ${newline} ${level} : ${message} ${exception:format=tostring}"
            };

            LogManager.Configuration.AddTarget(awsTarget);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, awsTarget));

            return services;
        }
    }
}
