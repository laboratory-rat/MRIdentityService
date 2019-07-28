using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Model.AppProvider.Access
{
    public class ProviderAccessDisplayModel
    {
        public List<ProviderAccessTokenModel> Tokens { get; set; }
        public List<ProviderAccessRuleModel> Rules { get; set; }
    }

    public class ProviderAccessTokenModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreatedByEmail { get; set; }
        public string CreatedById { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class ProviderAccessRuleModel
    {
        public bool IsAccessGranted { get; set; }
        public bool IsEnabled { get; set; }
        public List<string> IPS { get; set; }
        public List<string> Domains { get; set; }
    }
}
