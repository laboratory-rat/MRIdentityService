using Infrastructure.Entity.AppFile;
using Infrastructure.Model.Common;
using System.Collections.Generic;

namespace Infrastructure.Model.AppProvider
{
    public class ProviderUpdateModel
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public List<TranslationModel> Name { get; set; }
        public List<TranslationModel> Description { get; set; }
        public List<string> Categories { get; set; }
        public MRImage Avatar { get; set; }
        public MRImage Background { get; set; }
        public ProviderOptionsUpdateModel Options { get; set; }
    }

    public class ProviderOptionsUpdateModel
    {
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
        public bool IsRegistrationAvaliable { get; set; }
        public List<ProviderRoleUpdateModel> Roles { get; set; }
        public List<string> DefaultRoles { get; set; }
        public string CallbackUrl { get; set; }

    }

    public class ProviderRoleUpdateModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

}
