using Infrastructure.Model.AppUser;
using System.Threading.Tasks;

namespace Infrastructure.Interface.Manager
{
    public interface IManagerUserControl
    {
        Task<UserStatusModel> SignIn(UserSignInModel model);
        Task<UserStatusModel> AutoSignIn(UserAutoSignInModel model);
        Task<UserStatusModel> SignUp(UserSignupModel model);
        Task<bool> MailFree(string email);
        Task<bool> SignOut();
        Task<MRApiCommon.Infrastructure.Model.ApiResponse.OkApiResponse> SetLanguage(string languageCode);

        #region admin

        System.Collections.Generic.List<RoleDisplayModel> AvailableRoles();
        Task<MRApiCommon.Infrastructure.Model.ApiResponse.PaginationApiResponse<UserDisplayShortModel>> GetList(int skip, int limit, Model.Common.SortModel sort = null, UserSearchModel search = null);
        Task<System.Collections.Generic.List<RoleDisplayModel>> UpdateRoles(string id, UserUpdateRolesModel model);
        Task<UserDisplayShortModel> Profile();
        Task<UserDisplayShortModel> UpdateProfile(UserUpdateModel model);
        Task<Entity.AppFile.MRImage> UpdateAvatar(Entity.AppFile.MRImage image);
        Task<bool> UpdateEmail(UserUpdateEmailModel model);
        Task<bool> UpdatePassword(UserUpdatePasswordModel model);

        #endregion
    }
}
