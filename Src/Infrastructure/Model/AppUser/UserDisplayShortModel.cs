using MRApiCommon.Infrastructure.Enum;
using System;
using System.Collections.Generic;

namespace Infrastructure.Model.AppUser
{
    public class UserDisplayShortModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public MRUserSex Sex { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreateTime { get; set; }
        public List<string> Roles { get; set; }
        public string LanguageCode { get; set; }
    }
}
