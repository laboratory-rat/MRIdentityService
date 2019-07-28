using Infrastructure.Entity.AppLanguage;
using MRApiCommon.Infrastructure.Database;
using MRApiCommon.Infrastructure.Interface;
using System.Collections.Generic;

namespace Infrastructure.Entity.AppCategory
{
    public class Category : MREntity, IMREntity
    {
        public List<Translation> Name { get; set; }
        public string CreatedBy { get; set; }
    }
}
