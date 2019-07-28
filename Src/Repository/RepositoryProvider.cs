using System.Threading.Tasks;
using Infrastructure.Entity.AppProvider;
using Infrastructure.Interface.Repository;
using Microsoft.Extensions.Options;
using MRApiCommon.Infrastructure.Enum;
using MRApiCommon.Options;

namespace DL
{
    public class RepositoryProvider : Repository<Provider>, IRepositoryProvider
    {
        public RepositoryProvider(IOptions<MRDbOptions> settings) : base(settings) { }

        public async Task<Provider> GetByToken(string providerToken)
        {
            var q = _builder
                .Match(x => x.Access.Tokens, x => x.Value == providerToken);

            return await GetByQueryFirst(q);
        }

        public async Task<ProviderToken> UpdateAddToken(string providerId, ProviderToken entity)
        {
            var q = _builder
                .Eq(x => x.Id, providerId)
                .Eq(x => x.State, MREntityState.Active)
                .UpdateAddToSet(x => x.Access.Tokens, entity);

            await UpdateByQuery(q);
            return entity;
        }

        public async Task UpdateDeleteToken(string providerId, string tokenValue)
        {
            var q = _builder
                .Eq(x => x.Id, providerId)
                .Eq(x => x.State, MREntityState.Active)
                .UpdatePullWhere(x => x.Access.Tokens, x => x.Value == tokenValue);

            await UpdateByQuery(q);
        }

        public async Task<ProviderClient> AddClient(string providerId, ProviderClient client)
        {
            var q = _builder
                .Eq(x => x.Id, providerId)
                .UpdateAddToSet(x => x.Clients, client);

            await UpdateByQuery(q);
            return client;
        }

        public async Task UpdateClient(string providerId, ProviderClient client)
        {
            var q = _builder
                .Eq(x => x.Id, providerId)
                .Match(x => x.Clients, x => x.UserId == client.UserId)
                .UpdateSet(x => x.Clients[-1].Email, client.Email)
                .UpdateSet(x => x.Clients[-1].Roles, client.Roles);

            await UpdateByQuery(q);
        }

        public async Task AddClientLogin(string providerId, string userId, ProviderClientLogin login)
        {
            var q = _builder
                .Eq(x => x.Id, providerId)
                .Match(x => x.Clients, x => x.UserId == userId)
                .UpdatePush(x => x.Clients[-1].Logins, login);

            await UpdateByQuery(q);
        }
    }
}
