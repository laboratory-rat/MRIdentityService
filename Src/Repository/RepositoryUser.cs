using Infrastructure.Entity.AppUser;
using Microsoft.Extensions.Options;
using MRApiCommon.Infrastructure.IdentityExtensions.Interface;
using MRApiCommon.Infrastructure.IdentityExtensions.Store;
using MRApiCommon.Options;
using System.Threading.Tasks;

namespace DL
{
    public class RepositoryUser : MRUserStore<User>, IMRUserStore<User>
    {
        public RepositoryUser(IOptions<MRDbOptions> settings) : base(settings) { }

        public async Task AddProvider(string userId, UserProvider provider)
        {
            var q = _builder
                .Eq(x => x.Id, userId)
                .UpdateAddToSet(x => x.Providers, provider);

            await UpdateByQuery(q);
        }

        public async Task SetLanguage(string userId, string languageCode)
        {
            var q = _builder
                .Eq(x => x.Id, userId)
                .UpdateSet(x => x.LanguageCode, languageCode);

            await UpdateByQuery(q);
        }
    }
}
