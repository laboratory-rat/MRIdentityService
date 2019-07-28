using MRApiCommon.Infrastructure.Interface;

namespace Infrastructure.Interface.Repository
{
    public interface IRepository<TEntity> : IMRRepository<TEntity, string>
        where TEntity : class, IMREntity<string>, new()
    {
    }
}
