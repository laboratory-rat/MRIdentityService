using AutoMapper;
using DL;
using Infrastructure.Interface.Manager;
using Infrastructure.Interface.Repository;
using Infrastructure.Model.AppLanguage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ManagerLanguage : Manager, IManagerLanguage
    {
        protected readonly IRepositoryLanguage _repositoryLanguage;

        public ManagerLanguage(IHttpContextAccessor httpContextAccessor,
                               ILoggerFactory logger,
                               RepositoryRole repositoryRole,
                               RepositoryUser repositoryUser,
                               ManagerUser managerUser,
                               IMapper mapper,
                               IRepositoryLanguage repositoryLanguage) : base(httpContextAccessor, logger, repositoryRole, repositoryUser, managerUser, mapper)
        {
            _repositoryLanguage = repositoryLanguage;
        }

        public async Task<List<LanguageDisplayModel>> GetAll()
        {
            var languages = await _repositoryLanguage.Get(x => x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active);
            if(languages == null || !languages.Any())
            {
                return new List<LanguageDisplayModel>();
            }

            return _mapper.Map<List<LanguageDisplayModel>>(languages);
        }
    }
}
