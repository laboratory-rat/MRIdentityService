using System;

namespace MRPackage.Package.Email
{
    [Serializable]
    public class UserResetPasswordModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string ServiceName { get; set; }
        public string ResetLink { get; set; }
    }
}
