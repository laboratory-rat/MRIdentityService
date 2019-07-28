using MRPackage.Consts;
using System;
using System.Collections.Generic;

namespace MRPackage.Message
{
    [Serializable]
    public class EmailMessage
    {
        public EmailTypes Type { get; set; }
        public string Subject { get; set; }
        public string FromName { get; set; }
        public EmailUserMessage From { get; set; }
        public List<EmailUserMessage> To { get; set; }
        public List<EmailUserMessage> Cc { get; set; }
        public List<EmailUserMessage> Bcc { get; set; }
        public string EmailBody { get; set; }
    }

    [Serializable]
    public class EmailUserMessage
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsMale { get; set; }
        public string Email { get; set; }
    }
}
