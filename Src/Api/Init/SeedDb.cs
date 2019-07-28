using BLL;
using Infrastructure.Entity.AppLanguage;
using Infrastructure.Interface.Repository;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Init
{
    public static class SeedDb
    {
        public static async Task SeedDatabase(this IServiceCollection services)
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var provider = scope.ServiceProvider;
                var languageRepository = provider.GetRequiredService<IRepositoryLanguage>();
                await SeedLanguage(languageRepository);
            }
        }

        public static async Task SeedLanguage(IRepositoryLanguage repository)
        {
            string contentRootPath = Directory.GetCurrentDirectory();
            var JSON = File.ReadAllText(contentRootPath + "/Seed/Languages.json");

            var list = JsonConvert.DeserializeObject<List<Language>>(JSON);
            var toAdd = new List<Language>();
            foreach (var lang in list)
            {
                if(!await repository.Any(x => x.Code == lang.Code))
                {
                    toAdd.Add(lang);
                }
            }

            if (toAdd.Any())
            {
                await repository.Insert(toAdd);
            }
        }
    }
}
