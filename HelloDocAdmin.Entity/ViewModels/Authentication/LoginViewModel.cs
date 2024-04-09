using System.ComponentModel.DataAnnotations;

namespace HelloDocAdmin.Entity.ViewModels.Authentication
{
    public class LoginViewModel
    {
        [EmailAddress]
        [Required(ErrorMessage = "Enter Email...")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Password")]

        public string Password { get; set; }
    }
    public class ChangePassModel
    {
        public string Email { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string? pwdModified { get; set; }
    }

    public class ForgotPassword
    {
        [EmailAddress]
        public string Email { get; set; }
    }
    public class NewRegistration
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
