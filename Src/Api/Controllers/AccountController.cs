using Infrastructure.Entity.AppFile;
using Infrastructure.Interface.Manager;
using Infrastructure.Model.AppUser;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : Controller
    {
        protected readonly IManagerUserControl _managerUserControl;

        public AccountController(IManagerUserControl managerUserControl)
        {
            _managerUserControl = managerUserControl;
        }

        [Route("signup")]
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromBody] UserSignupModel model)
        {
            return Json(await _managerUserControl.SignUp(model));
        }

        [Route("login")]
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] UserSignInModel model)
        {
            return Json(await _managerUserControl.SignIn(model));
        }

        [Route("autologin")]
        [HttpPut]
        public async Task<IActionResult> AutoSignIn([FromBody] UserAutoSignInModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Json(await _managerUserControl.AutoSignIn(model));
        }

        [HttpGet]
        [Route("mailfree/{email}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            return Json(await _managerUserControl.MailFree(email));
        }

        [Route("language/{languageCode}")]
        [HttpPut]
        public async Task<IActionResult> SetLanguage(string languageCode)
        {
            return Json(await _managerUserControl.SetLanguage(languageCode));
        }

        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Json("Success");
        }

        #region profile

        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> GetProfile()
        {
            return Json(await _managerUserControl.Profile());
        }

        [HttpPut]
        [Route("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Json(await _managerUserControl.UpdateProfile(model));
        }

        [HttpPut]
        [Route("avatar")]
        public async Task<IActionResult> UpdateAvatar([FromBody] MRImage image)
        {
            return Json(await _managerUserControl.UpdateAvatar(image));
        }

        [HttpPut]
        [Route("email")]
        public async Task<IActionResult> UpdateEmail ([FromBody] UserUpdateEmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Json(await _managerUserControl.UpdateEmail(model));
        }

        [HttpPut]
        [Route("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UserUpdatePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return Json(await _managerUserControl.UpdatePassword(model));
        }

        #endregion

        #region Logins

        #endregion
    }
}
