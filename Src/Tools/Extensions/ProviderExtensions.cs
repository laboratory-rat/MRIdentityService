using Infrastructure.Entity.AppProvider;
using Infrastructure.Enum;
using System;
using System.Linq;

namespace Tools.Extensions
{
    public static class ProviderExtensions
    {
        public static readonly ProviderUserRole[] EditRoles = new ProviderUserRole[]
        {
            ProviderUserRole.ADMINISTRATOR,
            ProviderUserRole.EDIT,
            ProviderUserRole.OWNER
        };

        public static readonly ProviderUserRole[] DeleteRoles = new ProviderUserRole[]
        {
            ProviderUserRole.OWNER
        };

        public static bool IsEditAllow(this Provider entity, string userId)
            => entity != null
                && entity.Users != null
                && entity.Users.Any()
                && entity.Users.Any(eUser
                    => eUser.UserId == userId
                    && eUser.Roles != null
                    && eUser.Roles.Any()
                    && eUser.Roles.Any(eUserRole => EditRoles.Contains(eUserRole)));

        public static bool IsAllowToDelete(this Provider entity, string userId)
            => entity != null
                && entity.Users != null
                && entity.Users.Any()
                && entity.Users.Any(eUser
                    => eUser.UserId == userId
                    && eUser.Roles != null
                    && eUser.Roles.Any()
                    && eUser.Roles.Any(eUserRole => DeleteRoles.Contains(eUserRole)));
    }
}
