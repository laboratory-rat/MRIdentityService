using MRApiCommon.Infrastructure.IdentityExtensions.Components;
using MRApiCommon.Infrastructure.Interface;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entity.AppUser
{
    public class User : MRUser, IMREntity<string>
    {
        public List<UserProvider> Providers { get; set; }
        public string LanguageCode { get; set; }
    }

    public class UserProvider
    {
        public string ProviderId { get; set; }
        public string ProviderSlug { get; set; }
        public DateTimeOffset ConnectTime { get; set; }
        public List<string> Roles { get; set; }
    }
}
