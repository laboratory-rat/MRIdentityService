using Infrastructure.Model.AppCategory;
using System;
using System.Collections.Generic;

namespace Infrastructure.Model.AppProvider
{
    public class ProviderDisplayShortModel
    {
        public string Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public List<CategoryDisplayModel> Categories { get; set; }
        public string AvatarUrl { get; set; }
        public string BackgroundUrl { get; set; }
        public ProviderOptionsDisplayModel Options { get; set; }
    }

    public class ProviderOptionsDisplayModel
    {
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
        public bool IsRegistrationEnabled { get; set; }
    }
}
