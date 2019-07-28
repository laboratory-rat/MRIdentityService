using Infrastructure.Entity.AppCategory;
using Infrastructure.Entity.AppFile;
using Infrastructure.Entity.AppLanguage;
using System.Collections.Generic;

namespace Infrastructure.Entity.AppProvider
{
    public class Provider : AppEntity, IAppEntity
    {
        public string Slug { get; set; }
        public List<Translation> Name { get; set; }
        public List<Translation> Description { get; set; }
        public MRImage Avatar { get; set; }
        public List<ProviderUser> Users { get; set; }
        public MRImage Background { get; set; }
        public List<ProviderCategory> Categories { get; set; }
        public List<ProviderClient> Clients { get; set; }
        public ProviderOptions Options { get; set; }
        public ProviderAccess Access { get; set; }
    }
}
