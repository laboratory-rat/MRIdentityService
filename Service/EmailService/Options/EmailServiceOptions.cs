namespace EmailService.Options
{
    public class EmailServiceOptions
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 587;
        public EmailServiceEmailOptions Emails { get; set; }
    }

    public class EmailServiceEmailOptions
    {
        public string Support { get; set; }
        public string Bot { get; set; }
    }
}
