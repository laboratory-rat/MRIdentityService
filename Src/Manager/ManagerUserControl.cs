using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Connector.Bucket;
using BLL.Tools;
using DL;
using Infrastructure.Consts;
using Infrastructure.Entity.AppFile;
using Infrastructure.Entity.AppProvider;
using Infrastructure.Entity.AppUser;
using Infrastructure.Interface.Manager;
using Infrastructure.Interface.Repository;
using Infrastructure.Model.AppUser;
using Infrastructure.Model.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRApiCommon.Exception;
using MRApiCommon.Infrastructure.Enum;
using MRApiCommon.Infrastructure.Model.ApiResponse;
using MRApiCommon.Options;
using MRApiCommon.Service;
using Tools;

namespace BLL
{
    public class ManagerUserControl : Manager, IManagerUserControl
    {
        protected readonly IRepositoryProvider _repositoryProvider;
        protected readonly IRepositoryLanguage _repositoryLanguage;
        protected readonly MRTokenService _serviceToken;
        protected readonly MRTokenOptions _tokenOptions;
        protected readonly ConnectorBucketImageD _bucketD;
        protected readonly ConnectorBucketImageR _bucketR;
        protected readonly IRepositoryEmailChangeUnit _repositoryEmailChangeUnit;

        public ManagerUserControl(IHttpContextAccessor httpContextAccessor,
            ILoggerFactory logger,
            RepositoryRole repositoryRole,
            RepositoryUser repositoryUser,
            MRTokenService serviceToken,
            IOptions<MRTokenOptions> tokenOptions,
            ManagerUser managerUser,
            IMapper mapper,
            IRepositoryLanguage repositoryLanguage,
            IRepositoryProvider repositoryProvider,
            ConnectorBucketImageD bucketD,
            ConnectorBucketImageR bucketR,
            IRepositoryEmailChangeUnit repositoryEmailChangeUnit) : base(httpContextAccessor, logger, repositoryRole, repositoryUser, managerUser, mapper)
        {
            _repositoryProvider = repositoryProvider;
            _serviceToken = serviceToken;
            _tokenOptions = tokenOptions.Value;
            _repositoryLanguage = repositoryLanguage;
            _bucketD = bucketD;
            _bucketR = bucketR;
            _repositoryEmailChangeUnit = repositoryEmailChangeUnit;
        }

        public async Task<UserDisplayShortModel> Profile()
        {
            return _mapper.Map<UserDisplayShortModel>(await _repositoryUser.Get(_userId));
        }

        public async Task<UserDisplayShortModel> UpdateProfile(UserUpdateModel model)
        {
            var currentUser = _currentUser;
            _mapper.Map(model, currentUser);
            await _repositoryUser.Replace(currentUser);
            return _mapper.Map<UserDisplayShortModel>(await _repositoryUser.Get(currentUser.Id));
        }

        public async Task<MRImage> UpdateAvatar(MRImage image)
        {
            var rImage = await CopyToRBucket(image.Key);
            await _bucketD.Delete(image.Key);

            var currentUser = await _repositoryUser.Get(_userId);
            currentUser.Image = new MRApiCommon.Infrastructure.IdentityExtensions.Components.MRUserImage
            {
                Key = rImage.Key,
                Url = rImage.Url
            };

            await _repositoryUser.Replace(currentUser);
            return rImage;
        }

        public async Task<bool> UpdateEmail(UserUpdateEmailModel model)
        {
            var exists = await _repositoryUser.Any(x => x.Email == model.Email && x.State == MREntityState.Active);
            if (exists)
            {
                throw new MRException<object>((int)ExceptionCode.SYSTEM_EXCEPTION, "Email already in use");
            }

            var currentUser = await _repositoryUser.Get(_userId);
            if(!await _managerUser.CheckPasswordAsync(currentUser, model.Password))
            {
                throw new MRException<object>((int)ExceptionCode.ACCESS_DENIED, "Wrong password");
            }

            var entity = new EmailChangeUnit
            {
                NewEmail = model.Email,
                OldEmail = _userEmail,
                Status = Infrastructure.Entity.Enum.EmailChangeResult.NEW,
                Expired = DateTime.UtcNow.AddDays(3),
                Token = KeyGenerator.GenerateAccessKey(8),
                UserId = _userId,
            };

            await _repositoryEmailChangeUnit.Insert(entity);
            return true;
        }

        public async Task<bool> UpdatePassword(UserUpdatePasswordModel model)
        {
            var currentUser = await _repositoryUser.Get(_userId);
            if(!await _managerUser.CheckPasswordAsync(currentUser, model.Password))
            {
                _eAccessDenied("Incorrect password");
            }

            var result = await _managerUser.ChangePasswordAsync(currentUser, model.Password, model.NewPassword);
            return result.Succeeded;
        }

        #region signIn

        public async Task<UserStatusModel> SignIn(UserSignInModel model)
        {
            var user = await _managerUser.FindByEmailAsync(model.Email);
            if (user == null || !await _managerUser.CheckPasswordAsync(user, model.Password))
            {
                _eNotFound<UserStatusModel>("User not found");
            }

            var userRoles = await _managerUser.GetRolesAsync(user);
            var token = _serviceToken.GenerateToken(user.Id, user.Email, userRoles);

            UserLoginInfo li = null;
            string callbackToken = null;
            if (!string.IsNullOrWhiteSpace(model.ProviderSlug))
            {
                var provider = await _repositoryProvider.GetFirst(x => x.Slug == model.ProviderSlug && x.State == MREntityState.Active);
                if (provider != null && provider.Options.IsEnabled)
                {
                    li = new UserLoginInfo(provider.Id, token, provider.Slug);

                    _serviceToken.GenerateToken("SOME_SECRET_KEY", "MR_IDENTITY", "SOME_PROVIDER", 60, new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("provider.id", provider.Id),
                        new Tuple<string, string>("user.id", user.Id)
                    }, "name", "password");

                    callbackToken = ProviderTokenGenerator.Generate(provider.Id, user.Id);
                }
            }

            if (li == null)
            {
                li = new UserLoginInfo(_tokenOptions.Issuer, token, _tokenOptions.Issuer);
            }

            if(await _managerUser.FindByLoginAsync(li.LoginProvider, li.ProviderKey) != null)
            {
                await _managerUser.RemoveLoginAsync(user, li.LoginProvider, li.ProviderKey);
            }

            await _managerUser.AddLoginAsync(user, li);
            var isEmailOnChange = await _repositoryEmailChangeUnit.Any(x => x.State == MREntityState.Active && x.UserId == user.Id && x.Status == Infrastructure.Entity.Enum.EmailChangeResult.NEW);

            _logger.LogInformation($"User login");

            return new UserStatusModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles.ToList(),
                AccessToken = token,
                LanguageCode = user.LanguageCode,
                Email = user.Email,
                Sex = user.Sex,
                IsEmailConfirmed = user.IsEmailConfirmed,
                IsEmailOnChange = isEmailOnChange,
                CallbackUrl = model.CallbackUrl,
                CallbackToken = callbackToken
            };
        }

        public async Task<UserStatusModel> AutoSignIn(UserAutoSignInModel model)
        {
            var provider = await _repositoryProvider.GetFirst(x => x.Slug == model.ProviderSlug && x.State == MREntityState.Active);
            if(provider == null)
            {
                _eNotFound("Provider not found");
            }

            return new UserStatusModel
            {
                CallbackToken = ProviderTokenGenerator.Generate(provider.Id, _userId),
                CallbackUrl = string.IsNullOrWhiteSpace(model.RedirectUrl) ? provider.Options.CallbackUrl : model.RedirectUrl
            };
        }

        public async Task<UserStatusModel> SignUp(UserSignupModel model)
        {
            var exists = await _managerUser.FindByEmailAsync(model.Email);
            if (exists != null)
            {
                throw new MRException<UserStatusModel>((int)ExceptionCode.SYSTEM_EXCEPTION, "User with this email already exists");
            }

            var user = _mapper.Map<User>(model);
            var saveResult = await _managerUser.CreateAsync(user);

            if (!saveResult.Succeeded)
            {
                throw new MRException<UserStatusModel>((int)ExceptionCode.SYSTEM_EXCEPTION, "Can not create user");
            }

            await _managerUser.AddPasswordAsync(user, model.Password);
            await _managerUser.AddToRoleAsync(user, AppRoles.USER.ToString());

            string providerToken = null;

            if (!string.IsNullOrWhiteSpace(model.ProviderSlug))
            {
                var provider = await _repositoryProvider.GetFirst(x => x.Slug == model.ProviderSlug && x.State == MREntityState.Active);
                if(provider != null)
                {
                    if (!provider.Options.IsRegistrationAvaliable)
                    {
                        throw new MRException<object>(-1, "Provider registration disabled");
                    }

                    providerToken = ProviderTokenGenerator.Generate(provider.Id, user.Id);
                }
            }

            var token = _serviceToken.GenerateToken(user.Id, user.Email, new List<string> { AppRoles.USER.ToString() });

            var response = new UserStatusModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                AccessToken = token,
                Roles = new List<string> { AppRoles.USER.ToString() },
                LanguageCode = user.LanguageCode,
                Sex = user.Sex,
                CallbackUrl = model.CallbackUrl,
                CallbackToken = providerToken,
                IsEmailConfirmed = false,
                IsEmailOnChange = false
            };

            return response;
        }

        public async Task<bool> MailFree(string email)
        {
            return (await _managerUser.FindByEmailAsync(email)) == null;
        }

        public async Task<OkApiResponse> SetLanguage(string languageCode)
        {
            if (await _repositoryLanguage.Any(x => x.State == MRApiCommon.Infrastructure.Enum.MREntityState.Active && x.Code == languageCode))
            {
                await _repositoryUser.SetLanguage(_currentUser.Id, languageCode);
            }

            return new OkApiResponse();
        }

        public async Task<bool> SignOut()
        {
            await _managerUser.RemoveLoginAsync(_currentUser, _tokenOptions.Issuer, _tokenOptions.Issuer);
            return true;
        }

        #endregion

        #region admin

        public List<RoleDisplayModel> AvailableRoles()
            => Enum.GetNames(typeof(AppRoles)).Select(x => new RoleDisplayModel
            {
                Name = x
            }).ToList();

        public async Task<PaginationApiResponse<UserDisplayShortModel>> GetList(int skip, int limit, SortModel sort = null, UserSearchModel search = null)
        {
            sort = SortModel.Check(sort);

            var result = new PaginationApiResponse<UserDisplayShortModel>(skip, limit, 0, new List<UserDisplayShortModel>());
            result.Total = (int)await _repositoryUser.Count(x => x.State == MREntityState.Active);

            var entities = await _repositoryUser.GetSorted(x => x.State == MREntityState.Active, x => x.CreateTime, true, result.Skip, result.Take);
            if (entities != null && entities.Any())
            {
                result.List = _mapper.Map<List<UserDisplayShortModel>>(entities.ToList());
            }

            return result;
        }

        public async Task<List<RoleDisplayModel>> UpdateRoles(string id, UserUpdateRolesModel model)
        {
            var user = await _repositoryUser.GetFirst(x => x.Id == id && x.State == MREntityState.Active);
            if (user == null)
            {
                _eNotFound("User not found");
            }

            if (!model.Roles.Contains(AppRoles.USER.ToString()))
            {
                throw new MRException<object>((int)ExceptionCode.BAD_MODEL, $"{AppRoles.USER.ToString()} role is required");
            }

            var userRoles = await _managerUser.GetRolesAsync(user);
            var rolesToAdd = model.Roles.Where(x => !userRoles.Contains(x));
            var rolesToDelete = userRoles.Where(x => !model.Roles.Contains(x));

            if (rolesToAdd.Any())
            {
                await _managerUser.AddToRolesAsync(user, rolesToAdd);
            }

            if (rolesToDelete.Any())
            {
                await _managerUser.RemoveFromRolesAsync(user, rolesToDelete);
            }

            userRoles = await _managerUser.GetRolesAsync(user);

            return userRoles.Select(x => new RoleDisplayModel
            {
                Name = x
            }).ToList();
        }

        #endregion

        protected async Task<MRImage> CopyToRBucket(string key)
        {
            var bucketData = await _bucketD.Get(key);
            var contentTypeKey = nameof(MRImage.ContentType).ToLowerInvariant();
            var contentType = bucketData.Metadata[contentTypeKey];
            var fileName = bucketData.Metadata["name"];
            bucketData.Metadata.Remove(contentTypeKey);
            bucketData.Metadata.Remove("name");
            var imageData = await _bucketR.UploadWithName(fileName, bucketData.Content, contentType, bucketData.Metadata);
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
