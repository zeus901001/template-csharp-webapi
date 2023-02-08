using System.ComponentModel.DataAnnotations;

namespace csharp_webapi.Models.Auth
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Requiredfield")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Please enter your email.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Requiredfield")]
        [DataType(DataType.Password)]
        [Display(Name = "Please enter your password.")]
        public string? Password { get; set; }
    }
}
