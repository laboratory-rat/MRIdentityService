using MRApiCommon.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Model.AppUser
{
    public class UserUpdateModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public MRUserSex Sex { get; set; }
    }
}
