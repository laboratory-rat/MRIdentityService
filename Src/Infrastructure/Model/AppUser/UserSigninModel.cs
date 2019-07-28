using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Model.AppUser
{
    public class UserSignInModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(128, MinimumLength = 3)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(128, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }

        public string CallbackUrl { get; set; }

        public string ProviderSlug { get; set; }
    }
}
