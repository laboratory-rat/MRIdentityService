using MRApiCommon.Infrastructure.Database;
using MRApiCommon.Infrastructure.Interface;

namespace Infrastructure.Entity
{
    public class AppEntity : MREntity, IAppEntity { }

    public interface IAppEntity : IMREntity<string> { }
}
