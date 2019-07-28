using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Model.AppUser
{
    public class UserUpdateEmailModel
    {
        [Required]
        [StringLength(int.MaxValue, MinimumLength = 8)]
        public string Password { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
    }
}
