using System.ComponentModel.DataAnnotations;

namespace LearningKitCore.Models.Users.Registration
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "The User name cannot be empty.")]
        [Display(Name = "User name")]
        [MaxLength(100, ErrorMessage = "The User name cannot be longer than 100 characters.")]
        public string UserName
        {
            get;
            set;
        }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The Email address cannot be empty.")]
        [Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(254, ErrorMessage = "The Email address cannot be longer than 254 characters.")]
        public string Email
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The Password cannot be empty.")]
        [Display(Name = "Password")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        public string Password
        {
            get;
            set;
        }

        [DataType(DataType.Password)]
        [Display(Name = "Password confirmation")]
        [MaxLength(100, ErrorMessage = "The Password cannot be longer than 100 characters.")]
        [Compare("Password", ErrorMessage = "The entered passwords do not match.")]
        public string PasswordConfirmation
        {
            get;
            set;
        }

        [Display(Name = "First name")]
        [Required(ErrorMessage = "The First name cannot be empty.")]
        [MaxLength(100, ErrorMessage = "The First name cannot be longer than 100 characters.")]
        public string FirstName
        {
            get;
            set;
        }

        [Display(Name = "Last name")]
        [Required(ErrorMessage = "The Last name cannot be empty.")]
        [MaxLength(100, ErrorMessage = "The Last name cannot be longer than 100 characters.")]
        public string LastName
        {
            get;
            set;
        }
    }
}
