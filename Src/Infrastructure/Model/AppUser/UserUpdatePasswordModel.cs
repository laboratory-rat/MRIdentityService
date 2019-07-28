using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Model.AppUser
{
    public class UserUpdatePasswordModel
    {
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        [StringLength(int.MaxValue, MinimumLength = 8)]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        public string Confirm { get; set; }
    }
}
