using Infrastructure.Entity.AppCategory;
using Infrastructure.Interface.Repository;
using Microsoft.Extensions.Options;
using MRApiCommon.Options;

namespace DL
{
    public class RepositoryCategory : Repository<Category>, IRepositoryCategory
    {
        public RepositoryCategory(IOptions<MRDbOptions> settings) : base(settings)
        {
        }
    }
}
