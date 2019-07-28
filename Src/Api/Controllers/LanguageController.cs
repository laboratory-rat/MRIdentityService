using Infrastructure.Interface.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [AllowAnonymous]
    public class LanguageController : Controller
    {
        protected readonly IManagerLanguage _managerLanguage;

        public LanguageController(IManagerLanguage managerLanguage)
        {
            _managerLanguage = managerLanguage;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(await _managerLanguage.GetAll());
        }
    }
}
