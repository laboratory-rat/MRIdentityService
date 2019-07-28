using Infrastructure.Model.AppProvider;
using Infrastructure.Model.AppProvider.Access;
using Infrastructure.Model.AppUser;
using MRApiCommon.Infrastructure.Model.ApiResponse;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Manager
{
    public interface IManagerProvider
    {
        #region provider

        Task<PaginationApiResponse<ProviderDisplayShortModel>> GetAll(int skip, int limit, string languageCode = null, string search = null);
        Task<PaginationApiResponse<ProviderDisplayShortModel>> Get(int skip, int limit, string languageCode = null, string search = null);
        Task<ProviderDisplayModel> Get(string slug, string languageCode = null);
        Task<ProviderUpdateModel> Update(string id);
        Task<ProviderUpdateModel> Update(ProviderUpdateModel model);
        Task<bool> Delete(string id);

        #endregion

        #region access

        Task<ProviderAccessTokenModel> CreateToken(string providerId, ProviderAccessTokenCreateModel model);
        Task<ProviderAccessDisplayModel> GetAccess(string id);
        Task<bool> DeleteToken(string providerId, string tokenValue);

        #endregion

        #region login

        Task<UserPorviderProofResponseModel> LoginProof(string providerToken, string userToken);

        #endregion
    }
}
