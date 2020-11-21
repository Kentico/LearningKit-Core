using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using CMS.DataProtection;
using CMS.ContactManagement;
using CMS.Membership;
using CMS.Core;

using Kentico.Membership;

using LearningKitCore.Models.Users.Registration;


namespace LearningKitCore.Controllers.Users
{
    public class RegisterWithConsentController : Controller
    {
        private readonly IFormConsentAgreementService formConsentAgreementService;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationUserManager<ApplicationUser> userManager;
        private readonly IEventLogService eventLogService;

        private readonly ConsentInfo consent;

        /// <summary>
        /// Constructor.
        /// You can use a dependency injection container to initialize the consent agreement service.
        /// </summary>
        public RegisterWithConsentController(IFormConsentAgreementService formConsentAgreementService,
                                             SignInManager<ApplicationUser> signInManager,
                                             ApplicationUserManager<ApplicationUser> userManager,
                                             IEventLogService eventLogService,
                                             IConsentInfoProvider consentInfoProvider)
        {
            this.formConsentAgreementService = formConsentAgreementService;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.eventLogService = eventLogService;

            // Gets the related consent
            // Fill in the code name of the appropriate consent object in Xperience
            consent = consentInfoProvider.Get("SampleRegistrationConsent");
        }

        /// <summary>
        /// Basic action that displays the registration form.
        /// </summary>
        public IActionResult Register()
        {
            var model = new RegisterWithConsentViewModel
            {
                // Adds the consent text to the registration model
                ConsentShortText = consent.GetConsentText("en-US").ShortText,
                ConsentIsAgreed = false
            };

            return View("RegisterWithConsent", model);
        }

        /// <summary>
        /// Handles creation of new users and consent agreements when the registration form is submitted.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterWithConsentViewModel model)
        {
            // Validates the received user data based on the view model
            if (!ModelState.IsValid)
            {
                model.ConsentShortText = consent.GetConsentText("en-US").ShortText;
                return View("RegisterWithConsent", model);
            }

            // Prepares a new user entity using the posted registration data
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Enabled = true // Enables the new user directly
            };

            // Attempts to create the user in the Xperience database
            IdentityResult registerResult = IdentityResult.Failed();
            try
            {
                registerResult = await userManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                // Logs an error into the Xperience event log if the creation of the user fails
                eventLogService.LogException("MvcApplication", "UserRegistration", ex);
                ModelState.AddModelError(String.Empty, "Registration failed");
            }            

            // If the registration was not successful, displays the registration form with an error message
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.ConsentShortText = consent.GetConsentText("en-US").ShortText;

                return View("RegisterWithConsent", model);
            }

            // Creates a consent agreement if the consent checkbox was selected in the registration form
            if (model.ConsentIsAgreed)
            {
                // Gets the current contact
                var currentContact = ContactManagementContext.GetCurrentContact();

                // Creates an agreement for the specified consent and contact
                // Passes the UserInfo object of the new user as a parameter, which is used to map the user's values
                // to a new contact in cases where the contact parameter is null,
                // e.g. for visitors who have not given an agreement with the site's tracking consent.
                formConsentAgreementService.Agree(currentContact, consent, UserInfo.Provider.Get(user.Id));
            }

            // If the registration was successful, signs in the user and redirects to a different action
            await signInManager.PasswordSignInAsync(user, model.Password, true, false);
            return RedirectToAction("Index", "Home");
        }
    }
}