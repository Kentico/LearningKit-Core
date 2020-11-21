using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using CMS.Core;

using Kentico.Membership;

using LearningKitCore.Models.Users.Registration;


namespace LearningKitCore.Controllers
{
    public class EmailRegisterController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationUserManager<ApplicationUser> userManager;
        private readonly IMessageService messageService;

        public EmailRegisterController(ApplicationUserManager<ApplicationUser> userManager, 
                                       SignInManager<ApplicationUser> signInManager,
                                       IMessageService messageService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.messageService = messageService;
        }


        /// <summary>
        /// Basic action that displays the registration form.
        /// </summary>
        public IActionResult RegisterWithEmailConfirmation()
        {
            return View();
        }

        /// <summary>
        /// Creates new users when the registration form is submitted and sends the confirmation emails.
        /// Accepts parameters posted from the registration form via the RegisterViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWithEmailConfirmation([FromServices] IEventLogService eventLogService, RegisterViewModel model)
        {
            // Validates the received user data based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Prepares a new user entity using the posted registration data
            // The user is not enabled by default
            ApplicationUser user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
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
                foreach (IdentityError error in registerResult.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View(model);
            }

            // Generates a confirmation token for the new user
            string token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Prepares the URL of the confirmation link for the user (targets the "ConfirmUser" action)
            // Fill in the name of your controller
            string confirmationUrl = Url.Action(nameof(ConfirmUser), "EmailRegister", new { userId = user.Id, token = token }, protocol: Request.Scheme);

            // Creates and sends the confirmation email to the user's address
            await messageService.SendEmailAsync(user.Email, "Confirm your new account",
                String.Format("Please confirm your new account by clicking <a href=\"{0}\">here</a>", confirmationUrl));

            // Displays a view asking the visitor to check their email and confirm the new account
            return RedirectToAction(nameof(CheckYourEmail));
        }


        public IActionResult CheckYourEmail()
        {
            return View();
        }


        /// <summary>
        /// Action for confirming new user accounts. Handles the links that users click in confirmation emails.
        /// </summary>
        public async Task<IActionResult> ConfirmUser(string userId, string token)
        {
            IdentityResult confirmResult;

            ApplicationUser user = await userManager.FindByIdAsync(userId);

            try
            {
                // Verifies the confirmation parameters and enables the user account if successful
                confirmResult = await userManager.ConfirmEmailAsync(user, token);
            }
            catch (InvalidOperationException)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                confirmResult = IdentityResult.Failed(new IdentityError() { Description = "User not found." });
            }

            if (confirmResult.Succeeded)
            {
                // If the verification was successful, displays a view informing the user that their account was activated
                return View();
            }

            // Returns a view informing the user that the email confirmation failed
            return RedirectToAction(nameof(EmailConfirmationFailed));
        }
        

        public IActionResult EmailConfirmationFailed()
        {
            return View();
        }
    }
}