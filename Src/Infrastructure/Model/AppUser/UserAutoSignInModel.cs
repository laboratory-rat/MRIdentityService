using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Model.AppUser
{
    public class UserAutoSignInModel
    {
        [Required]
        public string ProviderSlug { get; set; }
        public string RedirectUrl { get; set; }

    }
}
