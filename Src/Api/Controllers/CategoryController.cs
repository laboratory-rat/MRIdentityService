using Infrastructure.Interface.Manager;
using Infrastructure.Model.AppCategory;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRApiCommon.Infrastructure.Model.ApiResponse;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class CategoryController : BaseController
    {
        protected readonly IManagerCategory _managerCategory;
        public CategoryController(IManagerCategory managerCategory)
        {
            _managerCategory = managerCategory;
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] CategoryUpdateModel model)
        {
            return Json(await _managerCategory.Update(model));
        }

        [HttpGet("update/{id}")]
        [ProducesResponseType(200, Type = typeof(CategoryUpdateModel))]
        public async Task<IActionResult> Get(string id)
        {
            return Json(await _managerCategory.Get(id));
        }

        [HttpGet("{skip}/{limit}/{languageCode?}")]
        [ProducesResponseType(200, Type = typeof(PaginationApiResponse<CategoryDisplayModel>))]
        public async Task<IActionResult> GetList(int skip, int limit, string languageCode = null, string search = null)
        {
            return Json(await _managerCategory.Get(skip, limit, languageCode, search));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> Delete(string id)
        {
            return Json(await _managerCategory.Delete(id));
        }
    }
}
