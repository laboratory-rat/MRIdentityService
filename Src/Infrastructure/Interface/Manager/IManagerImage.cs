using Infrastructure.Entity.AppFile;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Manager
{
    public interface IManagerImage
    {
        Task<MRImage> Upload(IFormFile file);
    }
}
