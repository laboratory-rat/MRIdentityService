using Infrastructure.Entity.AppUser;
using Infrastructure.Interface.Repository;
using Microsoft.Extensions.Options;
using MRApiCommon.Options;

namespace DL
{
    public class RepositoryEmailChangeUnit : Repository<EmailChangeUnit>, IRepositoryEmailChangeUnit
    {
        public RepositoryEmailChangeUnit(IOptions<MRDbOptions> settings) : base(settings) { }
    }
}
