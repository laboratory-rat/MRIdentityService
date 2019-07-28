using System;

namespace MRPackage.Package.Email
{
    [Serializable]
    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public string ServiceLink { get; set; }
        public string ServiceImage { get; set; }
        public string ServiceName { get; set; }
    }
}
