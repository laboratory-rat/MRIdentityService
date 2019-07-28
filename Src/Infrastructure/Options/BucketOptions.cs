namespace Infrastructure.Options
{
    public class BucketOptions
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Bucket { get; set; }
        public string Region { get; set; }
        public string BasePath { get; set; }
    }
}
