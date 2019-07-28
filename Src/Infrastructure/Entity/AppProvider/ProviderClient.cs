using System.Collections.Generic;

namespace Infrastructure.Entity.AppProvider
{
    public class ProviderClient
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<ProviderClientLogin> Logins { get; set; } = new List<ProviderClientLogin>();
    }
}
