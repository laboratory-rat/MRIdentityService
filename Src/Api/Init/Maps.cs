using AutoMapper;
using Infrastructure.Consts;
using Infrastructure.Entity.AppCategory;
using Infrastructure.Entity.AppLanguage;
using Infrastructure.Entity.AppProvider;
using Infrastructure.Entity.AppUser;
using Infrastructure.Model.AppCategory;
using Infrastructure.Model.AppLanguage;
using Infrastructure.Model.AppProvider;
using Infrastructure.Model.AppProvider.Access;
using Infrastructure.Model.AppUser;
using Infrastructure.Model.Common;
using System.Collections.Generic;
using System.Linq;
using Tools;

namespace Api.Init
{
    public class MapsInit : Profile
    {
        public MapsInit()
        {
            CreateMap<UserSignupModel, User>();
            CreateMap<Language, LanguageDisplayModel>();
            CreateMap<CategoryUpdateModel, Category>()
                .ReverseMap();
            CreateMap<Category, ProviderCategory>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name.ToList().SelectTranslation(null).Value));
            CreateMap<TranslationModel, Translation>()
                .ReverseMap();
            CreateMap<Category, CategoryDisplayModel>()
                .ForMember(x => x.Name, opt => opt.Ignore());

            // provider
            CreateMap<Provider, ProviderDisplayShortModel>()
                .ForMember(x => x.Name, opt => opt.Ignore())
                .ForMember(x => x.Categories, opt => opt.Ignore())
                .ForMember(x => x.AvatarUrl, opt => opt.MapFrom((src, dest) =>
                {
                    return src.Avatar?.Url;
                }))
                .ForMember(x => x.BackgroundUrl, opt => opt.MapFrom((src, dest) =>
                {
                    return src.Background?.Url;
                }));
            CreateMap<ProviderOptions, ProviderOptionsDisplayModel>();
            CreateMap<Provider, ProviderDisplayModel>()
                .IncludeBase<Provider, ProviderDisplayShortModel>()
                .ForMember(x => x.Description, opt => opt.Ignore());
            CreateMap<Provider, ProviderUpdateModel>()
                .ForMember(x => x.Categories, opt => opt.MapFrom(provider => provider.Categories.Select(x => x.Id).ToList()));
            CreateMap<ProviderUpdateModel, Provider>()
                .ForMember(x => x.Categories, opt => opt.Ignore())
                .ForMember(x => x.Users, opt => opt.Ignore());

            // provider options
            CreateMap<ProviderOptionsUpdateModel, ProviderOptions>()
                .ReverseMap();
            CreateMap<ProviderRoleUpdateModel, ProviderRole>()
                .ReverseMap();

            // provider access
            CreateMap<ProviderAccess, ProviderAccessDisplayModel>();
            CreateMap<ProviderToken, ProviderAccessTokenModel>()
                .ForMember(x => x.CreatedByEmail, opt => opt.Ignore())
                .ForMember(x => x.CreatedById, opt => opt.MapFrom(x => x.CreatedBy));
            CreateMap<ProviderAccessTokenCreateModel, ProviderToken>()
                .ForMember(x => x.CreatedBy, opt => opt.Ignore());
            CreateMap<ProviderRule, ProviderAccessRuleModel>()
                .ReverseMap();

            // user maps
            CreateMap<User, UserDisplayShortModel>()
                .ForMember(x => x.ImageUrl, opt => opt.MapFrom((sorce, dest) =>
                {
                    return sorce.Image?.Url;
                }))
                .ForMember(x => x.Roles, opt => opt.MapFrom((source, dest) =>
                {
                    return source.Roles?.Select(x => x.Name).ToList() ?? new List<string>();
                }));
            CreateMap<UserUpdateModel, User>();
            CreateMap<User, UserPorviderProofResponseModel>()
                .ForMember(x => x.AvatarUrl, opt => opt.MapFrom((source, target) =>
                {
                    return source.Image?.Url;
                }))
                .ForMember(x => x.Socials, opt => opt.Ignore())
                .ForMember(x => x.Roles, opt => opt.Ignore());
        }
    }
}
