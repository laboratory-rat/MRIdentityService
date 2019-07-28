using System.Collections.Generic;

namespace Infrastructure.Entity.AppProvider
{
    public class ProviderOptions
    {
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
        public bool IsRegistrationAvaliable { get; set; }
        public List<ProviderRole> Roles { get; set; }
        public List<string> DefaultRoles { get; set; }
        public string CallbackUrl { get; set; }
    }

    public class ProviderRole
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
