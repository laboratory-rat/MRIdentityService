using AutoMapper;
using BLL.Connector.Bucket;
using BLL.Tools;
using DL;
using Infrastructure.Consts;
using Infrastructure.Entity.AppFile;
using Infrastructure.Entity.AppProvider;
using Infrastructure.Enum;
using Infrastructure.Interface.Manager;
using Infrastructure.Interface.Repository;
using Infrastructure.Model.AppCategory;
using Infrastructure.Model.AppProvider;
using Infrastructure.Model.AppProvider.Access;
using Infrastructure.Model.AppUser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MRApiCommon.Exception;
using MRApiCommon.Infrastructure.Enum;
using MRApiCommon.Infrastructure.Model.ApiResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tools;
using Tools.Extensions;

namespace BLL
{
    public class ManagerProvider : Manager, IManagerProvider
    {
        protected readonly IRepositoryProvider _repositoryProvider;
        protected readonly IRepositoryCategory _repositoryCategory;
        protected readonly IRepositoryEmailChangeUnit _repositoryEmailChangeUnit;
        protected readonly ConnectorBucketImageR _connectorBucketImageR;
        protected readonly ConnectorBucketImageD _connectorBucketImageD;

        protected readonly ProviderUserRole[] EditRoles = new ProviderUserRole[]
        {
            ProviderUserRole.EDIT,
            ProviderUserRole.ADMINISTRATOR,
            ProviderUserRole.OWNER
        };

        public ManagerProvider(IHttpContextAccessor httpContextAccessor,
                               ILoggerFactory logger,
                               RepositoryRole repositoryRole,
                               RepositoryUser repositoryUser,
                               ManagerUser managerUser,
                               IMapper mapper,
                               IRepositoryCategory repositoryCategory,
                               IRepositoryProvider repositoryProvider,
                               ConnectorBucketImageR connectorBucketImageR,
                               ConnectorBucketImageD connectorBucketImageD,
                               IRepositoryEmailChangeUnit repositoryEmailChangeUnit) : base(httpContextAccessor, logger, repositoryRole, repositoryUser, managerUser, mapper)
        {
            _repositoryProvider = repositoryProvider;
            _repositoryCategory = repositoryCategory;
            _connectorBucketImageR = connectorBucketImageR;
            _connectorBucketImageD = connectorBucketImageD;
            _repositoryEmailChangeUnit = repositoryEmailChangeUnit;
        }

        #region admin

        public async Task<PaginationApiResponse<ProviderDisplayShortModel>> GetAll(int skip, int limit, string languageCode = null, string search = null)
        {
            var total = int.Parse((await _repositoryProvider.Count(x => x.State == MREntityState.Active)).ToString());
            var response = new PaginationApiResponse<ProviderDisplayShortModel>(skip, limit, total, new List<ProviderDisplayShortModel>());
            var entities = await _repositoryProvider.GetSorted(x => x.State == MREntityState.Active, x => x.CreateTime, true, skip, limit);

            foreach (var entity in entities)
            {
                var name = entity.Name?.SelectTranslation(languageCode);
                var model = _mapper.Map<ProviderDisplayShortModel>(entity);
                model.Name = name?.Value;
                model.Categories = new List<CategoryDisplayModel>();

                var categories = await _repositoryCategory.GetIn(x => x.Id, entity.Categories?.Select(x => x.Id));
                if (categories != null && categories.Any())
                {
                    foreach (var c in categories)
                    {
                        var translation = c.Name.SelectTranslation(languageCode);
                        model.Categories.Add(new CategoryDisplayModel
                        {
                            CreateTime = c.CreateTime,
                            Id = c.Id,
                            LanguageCode = languageCode,
                            Name = translation?.Value
                        });
                    }
                }

                response.List.Add(model);
            }

            return response;
        }

        public async Task<PaginationApiResponse<ProviderDisplayShortModel>> Get(int skip, int limit, string languageCode = null, string search = null)
        {
            var total = await _repositoryProvider.Count(x => x.State == MREntityState.Active && x.Options.IsVisible);
            var result = new PaginationApiResponse<ProviderDisplayShortModel>(skip, limit, (int)total, new List<ProviderDisplayShortModel>());

            var entities = await _repositoryProvider.GetSorted(x => x.State == MREntityState.Active && x.Options.IsVisible, x => x.CreateTime, true, skip, limit);
            if(entities != null && entities.Any())
            {
                foreach(var provider in entities)
                {
                    var model = _mapper.Map<ProviderDisplayShortModel>(provider);
                    model.Name = provider.Name.SelectTranslation(languageCode)?.Value;
                    model.Categories = new List<CategoryDisplayModel>();

                    var categoryIds = provider.Categories?.Select(x => x.Id).ToList()
                        ?? new List<string>();

                    if(categoryIds != null && categoryIds.Any())
                    {
                        var categories = (await _repositoryCategory.GetIn(x => x.Id, categoryIds))
                            .Where(x => x.State == MREntityState.Active);

                        if(categories != null && categories.Any())
                        {
                            foreach(var cat in categories)
                            {
                                var catModel = new CategoryDisplayModel
                                {
                                    CreateTime = cat.CreateTime,
                                    Id = cat.Id,
                                    LanguageCode = languageCode,
                                };
                                catModel.Name = cat.Name.SelectTranslation(languageCode)?.Value;
                                model.Categories.Add(catModel);
                            }
                        }
                    }

                    result.List.Add(model);
                }
            }

            return result;
        }

        public async Task<ProviderDisplayModel> Get(string slug, string languageCode = null)
        {
            var entity = await _repositoryProvider.GetFirst(x => x.Slug == slug.ToLowerInvariant() && x.State == MREntityState.Active);
            if (entity == null)
            {
                throw new MRException<ProviderDisplayModel>((int)ExceptionCode.NOT_FOUND, "Provider not found");
            }

            var result = _mapper.Map<ProviderDisplayModel>(entity);
            result.Categories = new List<CategoryDisplayModel>();
            result.Name = entity.Name.SelectTranslation(languageCode)?.Value;
            result.Description = entity.Description.SelectTranslation(languageCode)?.Value;

            var categoryIds = entity.Categories?.Select(x => x.Id).ToList() ?? new List<string>();
            if(categoryIds != null && categoryIds.Any())
            {
                var categories = (await _repositoryCategory.GetIn(x => x.Id, categoryIds))
                    .Where(x => x.State == MREntityState.Active);

                foreach(var cat in categories)
                {
                    var cName = cat.Name.SelectTranslation(languageCode)?.Value;
                    result.Categories.Add(new CategoryDisplayModel
                    {
                        CreateTime = cat.CreateTime,
                        Id = cat.Id,
                        LanguageCode = languageCode,
                        Name = cName
                    });
                }
            }

            return result;
        }

        public async Task<ProviderUpdateModel> Update(string id)
        {
            var entity = await _repositoryProvider.GetFirst(x => x.Id == id && x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active);
            if (entity == null)
            {
                throw new MRException<ProviderDisplayModel>((int)ExceptionCode.NOT_FOUND, "Provider not found");
            }

            var entityUser = entity.Users.FirstOrDefault(x => x.UserId == _currentUser.Id);
            if (entityUser == null)
            {
                throw _eNotFound<ProviderUpdateModel>("Provider not found");
            }

            if (!entityUser.Roles.Any(x => EditRoles.Contains(x)))
            {
                throw _eNotFound<ProviderUpdateModel>("Provider not found");
            }

            var model = _mapper.Map<ProviderUpdateModel>(entity);
            return model;
        }

        public async Task<ProviderUpdateModel> Update(ProviderUpdateModel model)
        {
            if (model == null)
            {
                throw new MRException<ProviderUpdateModel>((int)ExceptionCode.BAD_REQUEST);
            }

            if (model.Name?.SelectTranslation(null) == null || model.Description?.SelectTranslation(null) == null)
            {
                throw new MRException<ProviderUpdateModel>((int)ExceptionCode.BAD_MODEL, "Default translations not found");
            }

            model.Slug = model.Slug.ToLowerInvariant().Replace(" ", "_");
            var existsWithSlug = await _repositoryProvider.GetFirst(x => x.Slug == model.Slug && x.State == MREntityState.Active);
            if(existsWithSlug != null)
            {
                if(string.IsNullOrWhiteSpace(model.Id) || existsWithSlug.Id != model.Id)
                {
                    throw new MRException<ProviderUpdateModel>((int)ExceptionCode.BAD_MODEL, "Provider with this slug already exists");
                }
            }

            Provider entity = null;
            List<string> imagesToDelete = new List<string>();

            if (string.IsNullOrWhiteSpace(model.Id))
            {
                entity = _mapper.Map<Provider>(model);
                entity.CreateTime = DateTime.UtcNow;
                if (model.Categories == null)
                {
                    entity.Categories = new List<ProviderCategory>();
                }
                else
                {
                    var dbCategories = (await _repositoryCategory
                        .GetIn(x => x.Id, model.Categories))
                        .Where(x => x.State == MREntityState.Active);

                    entity.Categories = _mapper.Map<List<ProviderCategory>>(dbCategories);
                }
                // set images
                if (entity.Avatar != null && !string.IsNullOrWhiteSpace(entity.Avatar.Key))
                {
                    imagesToDelete.Add(entity.Avatar.Key);
                    entity.Avatar = await CopyToRBucket(entity.Avatar.Key);
                }

                if (entity.Background != null && !string.IsNullOrWhiteSpace(entity.Background.Key))
                {
                    imagesToDelete.Add(entity.Background.Key);
                    entity.Background = await CopyToRBucket(entity.Background.Key);
                }

                entity.Users = new List<ProviderUser>
                {
                    new ProviderUser
                    {
                        CreatedBy = _userId,
                        CreateTime = DateTime.UtcNow,
                        Roles = new List<ProviderUserRole>
                        {
                            ProviderUserRole.OWNER
                        },
                        UserEmail = _userEmail,
                        UserId = _userId
                    }
                };

                try
                {
                    entity = await _repositoryProvider.Insert(entity);
                }
                catch (Exception ex)
                {
                    var e = ex;
                }
            }
            else
            {
                entity = await _repositoryProvider.GetFirst(x => x.Id == model.Id && x.State == MREntityState.Active);
                if (entity == null)
                {
                    throw _eNotFound<ProviderUpdateModel>("Provider not found");
                }

                var oldAvatar = entity.Avatar;
                var oldBackground = entity.Background;

                _mapper.Map(model, entity);

                if (model.Categories == null)
                {
                    entity.Categories = new List<ProviderCategory>();
                }
                else
                {
                    var dbCategories = (await _repositoryCategory
                        .GetIn(x => x.Id, model.Categories))
                        .Where(x => x.State == MREntityState.Active);

                    entity.Categories = _mapper.Map<List<ProviderCategory>>(dbCategories);
                }

                imagesToDelete.Add(await UpdateEntityImage(oldAvatar, entity.Avatar));
                imagesToDelete.Add(await UpdateEntityImage(oldBackground, entity.Background));

                imagesToDelete = imagesToDelete.Where(x => x != null).ToList();
                entity.UpdateTime = DateTime.UtcNow;
            }

            ProviderUpdateModel result = null;
            try
            {
                result = _mapper.Map<ProviderUpdateModel>(entity);
            }
            catch (Exception ex)
            {
                var e = ex;
            }

            if (imagesToDelete != null && imagesToDelete.Any())
            {
                foreach (var imageToDelete in imagesToDelete)
                {
                    try
                    {
                        await _connectorBucketImageD.Delete(imageToDelete);
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            await _connectorBucketImageR.Delete(imageToDelete);
                        }
                        catch (Exception eex) { }
                    }
                }
            }

            return result;
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await _repositoryProvider.GetFirst(x => x.Id == id && x.State == MREntityState.Active);
            if (entity == null)
            {
                throw _eNotFound<Provider>("Provider not found");
            }

            if(!entity.IsAllowToDelete(_userId) && !_isCurrentUserAdmin)
            {
                _eAccessDenied("Access denied");
            }

            await _repositoryProvider.DeleteSoft(entity);
            return true;
        }
        
        #endregion

        #region access

        public async Task<ProviderAccessDisplayModel> GetAccess(string id)
        {
            var provider = await _repositoryProvider.GetFirst(x => x.Id == id && x.State == MREntityState.Active);
            if (provider == null)
            {
                _eNotFound("Provider not found");
            }

            if (!provider.IsEditAllow(_userId) && !_userRoles.Contains(AppRoles.ADMINISTRATOR.ToString()))
            {
                _eAccessDenied(typeof(ProviderAccess));
            }

            if (provider.Access == null)
            {
                provider.Access = new ProviderAccess();
                await _repositoryProvider.Replace(provider);
            }

            var model = _mapper.Map<ProviderAccessDisplayModel>(provider.Access);
            var userIds = model.Tokens?.GroupBy(x => x.CreatedById).Select(x => x.Key).ToList()
                ?? new List<string>();

            if (userIds.Any())
            {
                var targetUsers = await _repositoryUser.GetIn(x => x.Id, userIds);
                if (targetUsers != null && targetUsers.Any())
                {
                    foreach (var token in model.Tokens)
                    {
                        token.CreatedByEmail = targetUsers.FirstOrDefault(x => x.Id == token.CreatedById)?.Email ?? "Undefined";
                    }
                }
            }

            return model;
        }

        public async Task<ProviderAccessTokenModel> CreateToken(string providerId, ProviderAccessTokenCreateModel model)
        {
            var provider = await _repositoryProvider.GetFirst(x => x.Id == providerId && x.State == MREntityState.Active);
            if (provider == null)
            {
                _eNotFound("Provider not found");
            }

            if(!provider.IsEditAllow(_userId) && !_userRoles.Contains(AppRoles.ADMINISTRATOR.ToString()))
            {
                _eAccessDenied(typeof(Provider));
            }

            var entity = _mapper.Map<ProviderToken>(model);
            entity.CreatedBy = _userId;
            entity.CreateTime = DateTime.UtcNow;
            entity.Value = KeyGenerator.GenerateAccessKey(16);

            await _repositoryProvider.UpdateAddToken(providerId, entity);

            var result = _mapper.Map<ProviderAccessTokenModel>(entity);
            result.CreatedById = _userId;
            result.CreatedByEmail = _userEmail;

            return result;
        }

        public async Task<bool> DeleteToken(string providerId, string tokenValue)
        {
            var provider = await _repositoryProvider.GetFirst(x => x.Id == providerId && x.State == MREntityState.Active);
            if (provider == null)
            {
                _eNotFound("Provider not found");
            }

            if (!provider.IsEditAllow(_userId) && !_userRoles.Contains(AppRoles.ADMINISTRATOR.ToString()))
            {
                _eAccessDenied(typeof(Provider));
            }

            if (provider.Access?.Tokens == null || !provider.Access.Tokens.Any(x => x.Value == tokenValue))
            {
                _eNotFound("Access token not found");
            }

            await _repositoryProvider.UpdateDeleteToken(providerId, tokenValue);
            return true;
        }

        #endregion

        #region login

        public async Task<UserPorviderProofResponseModel> LoginProof(string providerToken, string userToken)
        {
            var provider = await _repositoryProvider.GetByToken(providerToken);
            if(provider == null)
            {
                throw new MRException<object>(-1, "Wrong provider token");
            }

            (var providerId, var userId) = ProviderTokenGenerator.DecryptToken(userToken);

            if (!provider.Id.Equals(providerId))
            {
                throw new MRException<object>(-1, "Wrong provider token");
            }

            var user = await _repositoryUser.GetFirst(x => x.Id == userId && x.State == MREntityState.Active);
            if(user == null)
            {
                throw new MRException<object>(-1, "User not found");
            }

            var providerLogin = new ProviderClientLogin
            {
                ProviderToken = providerToken,
                ClientToken = userToken,
                Time = DateTime.UtcNow
            };

            if(provider.Clients == null)
            {
                provider.Clients = new List<ProviderClient>();
                await _repositoryProvider.Replace(provider);
            }
            
            var providerClient = provider.Clients.FirstOrDefault(x => x.UserId == user.Id);
            if(providerClient == null)
            {
                providerClient = new ProviderClient
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = provider.Options.DefaultRoles,
                    Logins = new List<ProviderClientLogin> { providerLogin }
                };

                provider.Clients.Add(providerClient);

                await _repositoryProvider.AddClient(provider.Id, providerClient);
            }
            else
            {
                await _repositoryProvider.AddClientLogin(provider.Id, providerClient.UserId, providerLogin);
            }

            var model = _mapper.Map<UserPorviderProofResponseModel>(user);
            model.Roles = providerClient.Roles;
            model.IsEmailConfirmed = await _managerUser.IsEmailConfirmedAsync(user);
            model.IsEmailOnChange = await _repositoryEmailChangeUnit.Any(x => x.UserId == user.Id && x.Status == Infrastructure.Entity.Enum.EmailChangeResult.NEW);

            return model;
        }

        #endregion

        protected async Task<string> UpdateEntityImage(MRImage oldImage, MRImage newImage)
        {
            string result = null;

            if (oldImage != null && string.IsNullOrWhiteSpace(newImage?.Key))
            {
                result = oldImage.Key;
            }
            else if (oldImage == null && !string.IsNullOrWhiteSpace(newImage?.Key))
            {
                await CopyToRBucket(newImage.Key);
            }
            else if (oldImage != null && !string.IsNullOrWhiteSpace(newImage?.Key) && !oldImage.Key.Equals(newImage.Key))
            {
                result = oldImage.Key;
                await CopyToRBucket(newImage.Key);
            }

            return result;
        }

        protected async Task<MRImage> CopyToRBucket(string key)
        {
            var bucketData = await _connectorBucketImageD.Get(key);
            var contentTypeKey = nameof(MRImage.ContentType).ToLowerInvariant();
            var contentType = bucketData.Metadata[contentTypeKey];
            var fileName = bucketData.Metadata["name"];
            bucketData.Metadata.Remove(contentTypeKey);
            bucketData.Metadata.Remove("name");
            var imageData = await _connectorBucketImageR.UploadWithName(fileName, bucketData.Content, contentType, bucketData.Metadata);
            return new MRImage
            {
                Key = imageData.Key,
                Url = imageData.Url,
                ContentType = contentType,
                CreatedBy = _currentUser.Id,
                Height = int.Parse(bucketData.Metadata["height"]),
                Width = int.Parse(bucketData.Metadata["width"]),
                Metadata = bucketData.Metadata
            };
        }
    }
}
