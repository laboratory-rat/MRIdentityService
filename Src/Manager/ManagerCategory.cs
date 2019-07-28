using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DL;
using Infrastructure.Entity.AppCategory;
using Infrastructure.Interface.Manager;
using Infrastructure.Interface.Repository;
using Infrastructure.Model.AppCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRApiCommon.Exception;
using MRApiCommon.Infrastructure.Model.ApiResponse;
using Tools;

namespace BLL
{
    public class ManagerCategory : Manager, IManagerCategory
    {
        protected readonly IRepositoryCategory _repositoryCategory;

        public ManagerCategory(IHttpContextAccessor httpContextAccessor,
                               ILoggerFactory logger,
                               RepositoryRole repositoryRole,
                               RepositoryUser repositoryUser,
                               ManagerUser managerUser,
                               IMapper mapper,
                               IRepositoryCategory repositoryCategory) : base(httpContextAccessor, logger, repositoryRole, repositoryUser, managerUser, mapper)
        {
            _repositoryCategory = repositoryCategory;
        }

        public async Task<CategoryUpdateModel> Update(CategoryUpdateModel model)
        {
            if (!_isAuthorized)
                throw _eNotFound<CategoryUpdateModel>("Category not found");

            Category entity = null;
            if (string.IsNullOrWhiteSpace(model.Id))
                entity = _mapper.Map<Category>(model);
            else
            {
                entity = (await _repositoryCategory.GetFirst(x => x.Id == model.Id && x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active)
                    ?? throw _eNotFound<CategoryUpdateModel>("Category not found"));
                entity = _mapper.Map(model, entity);
            }

            entity = string.IsNullOrWhiteSpace(entity.Id)
                ? await _repositoryCategory.Insert(entity)
                : await _repositoryCategory.Replace(entity);

            return _mapper.Map<CategoryUpdateModel>(entity);
        }

        public async Task<CategoryUpdateModel> Get(string id)
        {
            if (!_isAuthorized || !await _repositoryCategory.Any(x => x.Id == id && x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active))
                throw _eNotFound<CategoryUpdateModel>("Category not found");

            return _mapper.Map<CategoryUpdateModel>(await _repositoryCategory.GetFirst(x => x.Id == id && x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active));
        }

        public async Task<PaginationApiResponse<CategoryDisplayModel>> Get(int skip, int limit, string languageCode = null, string search = null)
        {
            var result = new PaginationApiResponse<CategoryDisplayModel>(skip, limit, new List<CategoryDisplayModel>());
            var total = await _repositoryCategory.Count(x => x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active);
            result.Total = (int)total;

            var categories = await _repositoryCategory.GetSorted(x => x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active, x => x.CreateTime, true, result.Skip, result.Take);
            if (categories != null)
            {
                foreach (var c in categories)
                {
                    var translation = c.Name.SelectTranslation(languageCode);
                    result.List.Add(new CategoryDisplayModel
                    {
                        Id = c.Id,
                        LanguageCode = translation?.LanguageCode,
                        Name = translation?.Value
                    });
                }
            }

            return result;
        }

        public async Task<bool> Delete(string id)
        {
            if (!_isAuthorized || !await _repositoryCategory.Any(x => x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active && x.Id == id))
                throw _eNotFound("Category not found");

            await _repositoryCategory.DeleteSoftFirst(x => x.Id == id);
            return true;
        }
    }
}
