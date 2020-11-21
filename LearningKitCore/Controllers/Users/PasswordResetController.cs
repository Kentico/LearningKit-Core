using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using Kentico.Membership;

using LearningKitCore.Models.Users.PasswordReset;

namespace LearningKitCore.Controllers
{
    public class PasswordResetController : Controller
    {
        private readonly ApplicationUserManager<ApplicationUser> userManager;
        private readonly IMessageService messageService;


        public PasswordResetController(ApplicationUserManager<ApplicationUser> userManager,
                                 IMessageService messageService)
        {
            this.userManager = userManager;
            this.messageService = messageService;
        }

        /// <summary>
        /// Allows visitors to submit their email address and request a password reset.
        /// </summary>
        public IActionResult PasswordResetRequest()
        {
            return View();
        }

        /// <summary>
        /// Generates a password reset request for the specified email address.
        /// Accepts the email address posted via the RequestPasswordResetViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPasswordReset(PasswordResetRequestViewModel model)
        {
            // Validates the received email address based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Gets the user entity for the specified email address
            ApplicationUser user = await userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                // Generates a password reset token for the user
                string token = await userManager.GeneratePasswordResetTokenAsync(user);

                // Prepares the URL of the password reset link (targets the "ResetPassword" action)
                // Fill in the name of your controller
                string resetUrl = Url.Action(nameof(PasswordResetController.PasswordReset), nameof(PasswordResetController), new { userId = user.Id, token }, Request.Scheme);

                // Creates and sends the password reset email to the user's address
                await messageService.SendEmailAsync(user.Email, "Password reset request",
                    $"To reset your account's password, click <a href=\"{resetUrl}\">here</a>");
            }

            // Displays a view asking the visitor to check their email and click the password reset link
            return View("CheckYourEmail");
        }

        /// <summary>
        /// Handles the links that users click in password reset emails.
        /// If the request parameters are valid, displays a form where users can reset their password.
        /// </summary>
        public async Task<IActionResult> PasswordReset(int? userId, string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            ApplicationUser user = await userManager.FindByIdAsync(userId.ToString());

            try
            {
                // Verifies the parameters of the password reset request
                // True if the token is valid for the specified user, false if the token is invalid or has expired
                // By default, the generated tokens are single-use and expire in 1 day
                if (await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token))
                {
                    // If the password request is valid, displays the password reset form
                    var model = new PasswordReseViewModel
                    {
                        UserId = userId.Value,
                        Token = token
                    };

                    return View(model);
                }

                // If the password request is invalid, returns a view informing the user
                return View("PasswordResetResult", ViewBag.Success = false);
            }
            catch (InvalidOperationException)
            {
                // An InvalidOperationException occurs if a user with the given ID is not found
                // Returns a view informing the user that the password reset request is not valid
                return View("PasswordResetResult", ViewBag.Success = false);
            }
        }

        /// <summary>
        /// Resets the user's password based on the posted data.
        /// Accepts the user ID, password reset token and the new password via the ResetPasswordViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(PasswordReseViewModel model)
        {
            // Validates the received password data based on the view model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool result = false;

            ApplicationUser user = await userManager.FindByIdAsync(model.UserId.ToString());

            // Changes the user's password if the provided reset token is valid
            if (user != null && (await userManager.ResetPasswordAsync(user, model.Token, model.Password)).Succeeded)
            {
                // If the password change was successful, displays a message informing the user
                result = true;
            }

            // Occurs if the reset token is invalid
            // Returns a view informing the user that the password reset failed
            return View("PasswordResetResult", ViewBag.Success = result);            
        }
    }
}