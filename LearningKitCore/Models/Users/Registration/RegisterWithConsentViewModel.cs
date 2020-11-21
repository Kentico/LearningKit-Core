using System.ComponentModel.DataAnnotations;

namespace LearningKitCore.Models.Users.Registration
{
    public class RegisterWithConsentViewModel : RegisterViewModel
    {        
        public string ConsentShortText { get; set; }
        
        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must consent")]
        [Display(Name = "Consent with personal data processing")]
        public bool ConsentIsAgreed { get; set; }
    }
}