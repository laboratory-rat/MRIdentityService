using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Model.AppUser
{
    public class UserSignupModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(128, MinimumLength = 3)]
        public string Email { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public bool IsTermsConfirmed { get; set; }

        [StringLength(256, MinimumLength = 0)]
        public string ProviderSlug { get; set; }

        public string CallbackUrl { get; set; }
        public string LanguageCode { get; set; }
    }
}
