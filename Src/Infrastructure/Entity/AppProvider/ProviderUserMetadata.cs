namespace Infrastructure.Entity.AppProvider
{
    public class ProviderUserMetadata : AppEntity, IAppEntity
    {
        public string ProviderId { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public bool Success { get; set; }
        public int MyProperty { get; set; }
        public ProviderUserMetadataMeta Meta { get; set; }
    }


    public class ProviderUserMetadataMeta
    {
        public string IpAddress { get; set; }
        public string Domain { get; set; }
    }
}
