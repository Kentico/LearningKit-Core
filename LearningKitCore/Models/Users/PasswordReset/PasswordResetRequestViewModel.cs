using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LearningKitCore.Models.Users.PasswordReset
{ 
    public class PasswordResetRequestViewModel
    {
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
    }
}