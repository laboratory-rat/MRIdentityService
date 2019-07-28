using Infrastructure.Interface.Manager;
using Infrastructure.Model.AppProvider;
using Infrastructure.Model.AppProvider.Access;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProviderController : BaseController
    {
        protected readonly IManagerProvider _managerProvider;

        public ProviderController(IManagerProvider managerProvider)
        {
            _managerProvider = managerProvider;
        }

        [HttpGet]
        [Route("{slug}/{languageCode?}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string slug, string languageCode = null)
        {
            return Json(await _managerProvider.Get(slug, languageCode));
        }

        [HttpGet]
        [Route("{skip}/{limit}/{languageCode?}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetList(int skip, int limit, string languageCode = null, string search = null)
        {
            return Json(await _managerProvider.Get(skip, limit, languageCode, search));
        }

        #region admin

        [HttpGet]
        [Route("all/{skip}/{limit}/{languageCode?}")]
        public async Task<IActionResult> GetAll(int skip, int limit, string languageCode = null, string search = null)
        {
            return Json(await _managerProvider.GetAll(skip, limit, languageCode, search));
        }

        [HttpGet]
        [Route("update/{id}")]
        public async Task<IActionResult> UpdateGet(string id)
        {
            return Json(await _managerProvider.Update(id));
        }

        [HttpPut]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] ProviderUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Json(await _managerProvider.Update(model));
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> Delete(string id)
        {
            return Json(await _managerProvider.Delete(id));
        }

        #endregion

        #region access

        [HttpGet]
        [Route("access/{id}")]
        [ProducesResponseType(200, Type = typeof(ProviderAccessDisplayModel))]
        public async Task<IActionResult> GetAccess(string id)
        {
            return Json(await _managerProvider.GetAccess(id));
        }

        [HttpPost]
        [Route("access/{id}/token")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> CreateToken(string id, [FromBody] ProviderAccessTokenCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Json(await _managerProvider.CreateToken(id, model));
        }

        [HttpDelete]
        [Route("access/{id}/token/{value}")]
        [ProducesResponseType(200, Type = typeof(bool))]
        public async Task<IActionResult> DeleteToken(string id, string value)
        {
            return Json(await _managerProvider.DeleteToken(id, value));
        }

        #endregion

        #region Approve

        [HttpGet("login/{providerAccessKey}/{userAccessKey}")]
        [AllowAnonymous]
        public async Task<IActionResult> ProofLogin(string providerAccessKey, string userAccessKey)
        {
            return Json(await _managerProvider.LoginProof(providerAccessKey, userAccessKey));
        }

        #endregion
    }
}
