using Infrastructure.Entity.AppProvider;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Repository
{
    public interface IRepositoryProvider : IRepository<Provider>
    {
        /// <summary>
        /// Get provider by token value
        /// </summary>
        /// <param name="providerToken"></param>
        /// <returns></returns>
        Task<Provider> GetByToken(string providerToken);

        /// <summary>
        /// Add new Access Token
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<ProviderToken> UpdateAddToken(string providerId, ProviderToken entity);

        /// <summary>
        /// Delete access token
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="tokenValue"></param>
        /// <returns></returns>
        Task UpdateDeleteToken(string providerId, string tokenValue);

        /// <summary>
        /// Add client
        /// </summary>
        /// <param name="providerId">Provider id</param>
        /// <param name="client">New client</param>
        /// <returns></returns>
        Task<ProviderClient> AddClient(string providerId, ProviderClient client);

        /// <summary>
        /// Update provider client
        /// </summary>
        /// <param name="providerId">Target provider id</param>
        /// <param name="client">Client to update</param>
        /// <returns></returns>
        Task UpdateClient(string providerId, ProviderClient client);

        /// <summary>
        /// Add provider client login
        /// </summary>
        /// <param name="providerId">Target provider</param>
        /// <param name="userId">Target user</param>
        /// <param name="login">New user</param>
        /// <returns></returns>
        Task AddClientLogin(string providerId, string userId, ProviderClientLogin login);
    }
}
