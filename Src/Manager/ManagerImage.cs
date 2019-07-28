using AutoMapper;
using BLL.Connector.Bucket;
using DL;
using Infrastructure.Entity.AppFile;
using Infrastructure.Interface.Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRApiCommon.Exception;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BLL
{
    public class ManagerImage : Manager, IManagerImage
    {
        protected readonly ConnectorBucketImageD _bucketImageD;
        protected readonly ConnectorBucketImageR _bucketImageR;

        protected readonly string[] ALLOWED_TYPES = new string[]
        {
            "image/jpeg",
            "image/jpg",
            "image/png"
        };

        public ManagerImage(IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory logger,
                            RepositoryRole repositoryRole,
                            RepositoryUser repositoryUser,
                            ManagerUser managerUser,
                            IMapper mapper,
                            ConnectorBucketImageD bucketImageD,
                            ConnectorBucketImageR bucketImageR) : base(httpContextAccessor, logger, repositoryRole, repositoryUser, managerUser, mapper)
        {
            _bucketImageD = bucketImageD;
            _bucketImageR = bucketImageR;
        }

        public async Task<MRImage> Upload(IFormFile file)
        {
            if (!_isAuthorized)
            {
                throw new MRException<MRImage>((int)ExceptionCode.ACCESS_DENIED, "Access denied");
            }

            if(file == null)
            {
                throw new MRException<MRImage>((int)ExceptionCode.BAD_REQUEST, "Bad request");
            }

            if (!ALLOWED_TYPES.Contains(file.ContentType))
            {
                throw new MRException<MRImage>((int)ExceptionCode.BAD_REQUEST, "Not allowed");
            }

            var content = new byte[file.Length];
            using (var reader = file.OpenReadStream())
            {
                await reader.ReadAsync(content, 0, content.Length);
            }

            var response = new MRImage
            {
                CreatedBy = _currentUser.Id,
                ContentType = file.ContentType,
            };

            using (var image = Image.FromStream(file.OpenReadStream()))
            {
                response.Height = image.Height;
                response.Width = image.Width;
            }

            var bucketResponse = await _bucketImageD.UploadWithName(file.FileName, content, file.ContentType, new Dictionary<string, string>
            {
                { nameof(MRImage.Height).ToUpperInvariant(), response.Height.ToString() },
                { nameof(MRImage.Width).ToUpperInvariant(), response.Width.ToString() },
                { nameof(MRImage.CreatedBy).ToUpperInvariant(), response.CreatedBy.ToString() },
            });

            response.Key = bucketResponse.Key;
            response.Url = bucketResponse.Url;

            return response;
        }
    }
}
