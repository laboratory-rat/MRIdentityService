using MRPackage.Package.Email;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MRPackage.Consts
{
    public static class EmailTypeToModel
    {
        public static Dictionary<EmailTypes, Type> ConvertLib = new Dictionary<EmailTypes, Type>
        {
            { EmailTypes.REGISTRATION, typeof(UserRegistrationModel) },
            { EmailTypes.PASSWORD_RESET, typeof(UserResetPasswordModel) },
            { EmailTypes.EMAIL_CONFIRM, typeof(UserConfirmEmailModel) },
        };

        public static Dictionary<EmailTypes, string> PathLib = new Dictionary<EmailTypes, string>
        {
            { EmailTypes.REGISTRATION, "Registration" },
            { EmailTypes.PASSWORD_RESET, "ResetPassword" },
            { EmailTypes.EMAIL_CONFIRM, "ConfirmEmail" },
        };

        public static object Convert(EmailTypes type, string body)
        {
            if (ConvertLib.ContainsKey(type))
            {
                return JsonConvert.DeserializeObject(body, ConvertLib[type]);
            }

            return null;
        }
        public static EmailTemplateNameModel TemplateNames(EmailTypes type)
        {
            if (!PathLib.ContainsKey(type))
            {
                throw new ArgumentOutOfRangeException($"Can not find type {type.ToString()} in Template names collection");
            }

            return new EmailTemplateNameModel(PathLib[type]);
        }
    }

    public class EmailTemplateNameModel
    {
        public string Template { get; set; }
        public string Text { get; set; }

        public EmailTemplateNameModel(string basic)
        {
            Template = $"{basic}.cshtml";
            Text = $"{basic}.text.cshtml";
        }
    }
}
