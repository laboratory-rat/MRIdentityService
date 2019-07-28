using System.Collections.Generic;

namespace Infrastructure.Model.AppUser
{
    public class UserSearchModel
    {
        public string Name { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Providers { get; set; }
    }
}
