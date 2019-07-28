using Infrastructure.Model.AppLanguage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Manager
{
    public interface IManagerLanguage
    {
        Task<List<LanguageDisplayModel>> GetAll();
    }
}
