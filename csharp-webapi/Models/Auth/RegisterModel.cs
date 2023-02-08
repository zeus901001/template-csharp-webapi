using System.ComponentModel.DataAnnotations;

namespace csharp_webapi.Models.Auth
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "RequiredField")]
        [Display(Name = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        [Display(Name = "Please enter your password.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        [Display(Name = "Please confirm your password.")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        // [StringLength(12, MinimumLength = 4, ErrorMessage = "FirstNameLength")]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "EnglishCharsOnly")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "RequiredField")]
        // [StringLength(12, MinimumLength = 4, ErrorMessage = "LastNameLength")]
        [RegularExpression(@"^[a-zA-Z0-9_-]*$", ErrorMessage = "EnglishCharsOnly")]
        public string? LastName { get; set; }
    }
}
