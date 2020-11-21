using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LearningKitCore.Models.Users.Account
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Please enter a user name")]
        [Display(Name = "User name")]
        [MaxLength(100, ErrorMessage = "The User name cannot be longer than 100 characters.")]
        public string UserName { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MaxLength(100, ErrorMessage = "The password cannot be longer than 100 characters.")]
        public string Password { get; set; }


        [Display(Name = "Stay signed in")]
        public bool RememberMe { get; set; }
    }
}