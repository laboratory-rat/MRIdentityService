using Infrastructure.Entity.AppLanguage;
using Infrastructure.Interface.Repository;
using Microsoft.Extensions.Options;
using MRApiCommon.Options;

namespace DL
{
    public class RepositoryLanguage : Repository<Language>, IRepositoryLanguage
    {
        public RepositoryLanguage(IOptions<MRDbOptions> settings) : base(settings)
        {
        }
    }
}
