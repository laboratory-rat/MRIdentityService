using Infrastructure.Entity.AppUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MRApiCommon.Infrastructure.IdentityExtensions.Interface;
using MRMongoTools.Extensions.Identity.Manager;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class ManagerUser : MRUserManager<User>
    {
        public ManagerUser(IMRUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
                           IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
                           IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
                           IdentityErrorDescriber errors, IServiceProvider services,
                           ILogger<UserManager<User>> logger) : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }
    }
}
