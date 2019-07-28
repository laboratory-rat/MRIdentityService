using EmailService.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MRPackage.Consts;
using MRPackage.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TemplateService.Infrastructure.Interface;

namespace EmailService
{
    public class EmailSender
    {
        protected readonly EmailServiceOptions _options;
        protected readonly ILogger _logger;
        protected readonly ITemplateBuilder _templateBuilder;
        protected MailboxAddress _supportAddress => new MailboxAddress("Mad Rat Bot", "madratbot@gmail.com");

        public EmailSender(IOptions<EmailServiceOptions> options, ITemplateBuilder templateBuilder, ILogger<EmailSender> logger)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(EmailServiceOptions));
            _templateBuilder = templateBuilder ?? throw new ArgumentNullException(nameof(ITemplateBuilder));
            _logger = logger;
        }
        
        public async Task<MimeMessage> Send(EmailMessage message)
        {
            var model = EmailTypeToModel.Convert(message.Type, message.EmailBody);
            var templateNames = EmailTypeToModel.TemplateNames(message.Type);

            var mail = new MimeMessage();
            mail.From.Add(_supportAddress);

            message.To = message.To ?? new List<EmailUserMessage>();
            message.Cc = message.Cc ?? new List<EmailUserMessage>();
            message.Bcc = message.Bcc ?? new List<EmailUserMessage>();

            mail.To.AddRange(message.To.Select(x => new MailboxAddress($"{x.FirstName} {x.LastName}", x.Email)));
            mail.Cc.AddRange(message.Cc.Select(x => new MailboxAddress($"{x.FirstName} {x.LastName}", x.Email)));
            mail.Bcc.AddRange(message.Bcc.Select(x => new MailboxAddress($"{x.FirstName} {x.LastName}", x.Email)));

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = await _templateBuilder.Render(templateNames.Template, model);
            bodyBuilder.TextBody = await _templateBuilder.Render(templateNames.Text, model);

            mail.Body = bodyBuilder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_options.Host, _options.Port);
                    await client.AuthenticateAsync(_options.User, _options.Password);
                    _logger.LogInformation($"Send [{message.Type.ToString()}] to {message.To.First().Email} from {mail.From.First().Name}");

                    await client.SendAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can not send email.");
            }

            return mail;
        }
    }
}
