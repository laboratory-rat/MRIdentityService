namespace Infrastructure.Options
{
    public class AwsLogOptions
    {
        public string Region { get; set; }
        public string TargetGroup { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
