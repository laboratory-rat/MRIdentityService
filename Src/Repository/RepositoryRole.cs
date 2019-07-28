using Infrastructure.Interface.Repository;
using Microsoft.Extensions.Options;
using MRApiCommon.Infrastructure.IdentityExtensions.Store;
using MRApiCommon.Options;

namespace DL
{
    public class RepositoryRole : MRRoleStore, IRepositoryRole
    {
        public RepositoryRole(IOptions<MRDbOptions> settings) : base(settings) { }
    }
}
