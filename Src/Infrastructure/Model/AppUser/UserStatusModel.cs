using MRApiCommon.Infrastructure.Enum;
using System.Collections.Generic;

namespace Infrastructure.Model.AppUser
{
    public class UserStatusModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public MRUserSex Sex { get; set; }
        public List<string> Roles { get; set; }
        public string AccessToken { get; set; }
        public string LanguageCode { get; set; }
        public string CallbackUrl { get; set; }
        public string CallbackToken { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsEmailOnChange { get; set; }
        public bool DataCompleted => !string.IsNullOrWhiteSpace(FirstName + LastName) && Sex != MRUserSex.UNDEFINED;
    }
}
