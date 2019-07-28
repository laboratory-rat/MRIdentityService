using System;

namespace Infrastructure.Entity.AppProvider
{
    public class ProviderClientLogin
    {
        public string ProviderToken { get; set; }
        public string ClientToken { get; set; }
        public DateTime Time { get; set; }
    }
}
