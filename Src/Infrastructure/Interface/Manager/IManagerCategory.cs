using Infrastructure.Model.AppCategory;
using MRApiCommon.Infrastructure.Model.ApiResponse;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Manager
{
    public interface IManagerCategory
    {
        Task<CategoryUpdateModel> Update(CategoryUpdateModel model);
        Task<CategoryUpdateModel> Get(string id);
        Task<PaginationApiResponse<CategoryDisplayModel>> Get(int skip, int limit, string languageCode = null, string search = null);
        Task<bool> Delete(string id);
    }
}
