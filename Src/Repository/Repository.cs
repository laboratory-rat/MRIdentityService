using Infrastructure.Interface.Repository;
using Microsoft.Extensions.Options;
using MRApiCommon.Infrastructure.Database;
using MRApiCommon.Infrastructure.Interface;
using MRApiCommon.Options;

namespace DL
{
    public abstract class Repository<TEntity> : MRMongoRepository<TEntity, string>, IRepository<TEntity>
        where TEntity : class, IMREntity<string>, new()
    {
        public Repository(IOptions<MRDbOptions> settings) : base(settings) { }
    }
}
