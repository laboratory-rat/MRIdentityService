using System.Collections.Generic;

namespace Infrastructure.Model.AppProvider.Access
{
    class ProviderAccessRuleCreateModel
    {
        public bool IsAccessGranted { get; set; }
        public bool IsEnabled { get; set; }
        public List<string> IPS { get; set; }
        public List<string> Domains { get; set; }
    }
}
