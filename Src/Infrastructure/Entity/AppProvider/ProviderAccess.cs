using System;
using System.Collections.Generic;

namespace Infrastructure.Entity.AppProvider
{
    public class ProviderAccess
    {
        public List<ProviderToken> Tokens { get; set; } = new List<ProviderToken>();
        public List<ProviderRule> Rules { get; set; } = new List<ProviderRule>();
    }

    public class ProviderToken
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; }
    }

    public class ProviderRule
    {
        public bool IsAccessGranted { get; set; } = false;
        public bool IsEnabled { get; set; } = false;
        public List<string> IPS { get; set; } = new List<string>();
        public List<string> Domains { get; set; } = new List<string>();
    }
}
