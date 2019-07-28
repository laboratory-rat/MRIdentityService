using System.Collections.Generic;

namespace Infrastructure.Model.AppUser
{
    public class UserPorviderProofResponseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> Roles { get; set; }
        public string LanguageCode { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsEmailOnChange { get; set; }
        public List<UserProviderProofResponseSocialModel> Socials { get; set; }
    }

    public class UserProviderProofResponseSocialModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
