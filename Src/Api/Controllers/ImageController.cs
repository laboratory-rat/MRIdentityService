using Infrastructure.Entity.AppFile;
using Infrastructure.Interface.Manager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class ImageController : Controller
    {
        protected readonly IManagerImage _managerImage;

        public ImageController(IManagerImage managerImage)
        {
            _managerImage = managerImage;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(MRImage))]
        public async Task<IActionResult> Post(IFormFile file)
        {
            return Json(await _managerImage.Upload(file));
        }
    }
}
