using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;     

using CMS.Core;

using Kentico.Membership;

using LearningKitCore.Models.Users.Registration;
using CMS.EventLog;

namespace LearningKitCore.Controllers
{
    public class RegisterController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationUserManager<ApplicationUser> userManager;
        private readonly IEventLogService eventLogService;

        public RegisterController(ApplicationUserManager<ApplicationUser> userManager,
                                  SignInManager<ApplicationUser> signInManager,
                                  IEventLogService eventLogService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.eventLogService = eventLogService;
        }

        /// <summary>
        /// Basic action that displays the registration form.
        /// </summary>
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Handles creation of new users when the registration form is submitted.
        /// Accepts parameters posted from the registration form via the RegisterViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validates the received user data based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
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
            IdentityResult registerResult = new IdentityResult();
            try
            {
                registerResult = await userManager.CreateAsync(user, model.Password);
            }
            catch (Exception ex)
            {
                // Logs an error into the Xperience event log if the creation of the user fails
                eventLogService.LogException("MvcApplication", "UserRegistration", ex);
                ModelState.AddModelError(String.Empty, "Registration failed, see the event log for more details.");
            }
            
            if (registerResult.Succeeded)
            {
                // If the registration was successful and the user can sign in, signs in the user
                var signInResult =  await signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);

                // Redirects to a different action
                return RedirectToAction(nameof(HomeController.Users), "Home");                
            }

            // If the registration was not successful, displays the registration form with an error message
            foreach (IdentityError error in registerResult.Errors)
            {
                ModelState.AddModelError(String.Empty, error.Description);
            }
            return View(model);
        }
    }
}