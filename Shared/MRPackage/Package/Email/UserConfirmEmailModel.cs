using System;

namespace MRPackage.Package.Email
{
    [Serializable]
    public class UserConfirmEmailModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string ConfirmLink { get; set; }
    }
}
