using Infrastructure.Model.Common;
using System.Collections.Generic;

namespace Infrastructure.Model.AppCategory
{
    public class CategoryUpdateModel
    {
        public string Id { get; set; }
        public List<TranslationModel> Name { get; set; }
    }
}
