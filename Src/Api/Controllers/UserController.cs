using Infrastructure.Interface.Manager;
using Infrastructure.Model.AppUser;
using Infrastructure.Model.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMINISTRATOR")]
    [Route("[controller]")]
    public class UserController : BaseController
    {
        protected readonly IManagerUserControl _manager;

        public UserController(IManagerUserControl manager)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        [HttpGet]
        [Route("roles")]
        public IActionResult GetAvailableRoles()
        {
            return Json(_manager.AvailableRoles());
        }

        [HttpGet]
        [Route("list/{skip}/{limit}")]
        public async Task<IActionResult> GetList(int skip, int limit, [FromQuery] SortModel sort = null, [FromQuery] UserSearchModel search = null)
        {
            return Json(await _manager.GetList(skip, limit, sort, search));
        }

        [HttpPut]
        [Route("{id}/roles")]
        public async Task<IActionResult> UpdateRoles(string id, [FromBody] UserUpdateRolesModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Json(await _manager.UpdateRoles(id, model));
        }
    }
}
