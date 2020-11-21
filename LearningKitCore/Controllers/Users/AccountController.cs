//DocSection:Using
using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using CMS.Base;
using CMS.Core;
using CMS.Membership;
using CMS.Base.UploadExtensions;

using Kentico.Membership;

using LearningKitCore.Models.Users.Account;
//EndDocSection:Using

namespace LearningKitCore.Controllers
{
    public class AccountController : Controller
    {
        //DocSection:DependencyInjection
        private readonly IAvatarService avatarService;
        private readonly ApplicationUserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ISiteService siteService;
        private readonly IEventLogService eventLogService;

        public AccountController(ApplicationUserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 ISiteService siteService,
                                 IAvatarService avatarService,
                                 IEventLogService eventLogService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.siteService = siteService;
            this.avatarService = avatarService;
            this.eventLogService = eventLogService;
        }
        //EndDocSection:DependencyInjection


        //DocSection:SignIn
        /// <summary>
        /// Basic action that displays the sign-in form.
        /// </summary>
        public IActionResult SignIn()
        {
            return View();
        }


        /// <summary>
        /// Handles authentication when the sign-in form is submitted. Accepts parameters posted from the sign-in form via the SignInViewModel.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel model, string returnUrl)
        {
            // Validates the received user credentials based on the view model
            if (!ModelState.IsValid)
            {
                // Displays the sign-in form if the user credentials are invalid
                return View();
            }
               
            // Attempts to authenticate the user against the Xperience database
            var signInResult = Microsoft.AspNetCore.Identity.SignInResult.Failed;
            try
            {
                signInResult = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);
            }
            catch (Exception ex)
            {
                // Logs an error into the Xperience event log if the authentication fails
                eventLogService.LogException("MvcApplication", "AUTHENTICATION", ex);
            }
            
            // If the authentication was successful, redirects to the return URL when possible or to a different default action
            if (signInResult.Succeeded)
            {
                string decodedReturnUrl = WebUtility.UrlDecode(returnUrl);
                if (!string.IsNullOrEmpty(decodedReturnUrl) && Url.IsLocalUrl(decodedReturnUrl))
                {
                    return Redirect(decodedReturnUrl);
                }
                // Redirects the user to the homepage
                return RedirectToAction(nameof(HomeController.Users), "Home");
            }

            if(signInResult.IsNotAllowed)
            {
                // If the 'Registration requires administrator's approval' setting is enabled and the user account
                // is pending activation, displays an appropriate message
                ApplicationUser user = await userManager.FindByNameAsync(model.UserName);
                if (user != null && user.WaitingForApproval)
                {
                    ModelState.AddModelError(String.Empty, "You account is pending administrator approval.");

                    return RedirectToAction(nameof(WaitingForApproval));
                }

                // The other setting that causes 'IsNotAllowed' is 'Require email confirmation'
                ModelState.AddModelError(String.Empty, "Please confirm your email.");

                return View();
            }

            // If the authentication was not successful due to any other reason, displays the sign-in form with an "Authentication failed" message 
            ModelState.AddModelError(String.Empty, "Authentication failed, please verify that you entered the correct authentication credentials.");
            return View();            
        }


        public IActionResult WaitingForApproval()
        {
            return View();
        }
        //EndDocSection:SignIn


        //DocSection:Signout
        /// <summary>
        /// Action for signing out users. The Authorize attribute allows the action only for users who are already signed in.
        /// </summary>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignOut()
        {
            // Signs out the current user
            signInManager.SignOutAsync();

            // Redirects to a different action after the sign-out
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        //EndDocSection:Signout


        //DocSection:EditUser
        /// <summary>
        /// Displays a form where user information can be changed.
        /// </summary>
        public async Task<IActionResult> EditUser(bool avatarUpdateFailed = false)
        {
            // Finds the user based on their current user name
            ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);

            EditUserAccountViewModel model = new EditUserAccountViewModel()
            {
                User = user,
                AvatarUpdateFailed = avatarUpdateFailed
            };

            return View(model);
        }


        /// <summary>
        /// Saves the entered changes of the user details to the database.
        /// </summary>
        /// <param name="returnedUser">User that is changed.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(ApplicationUser returnedUser)
        {
            // Finds the user based on their current user name
            ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);

            // Assigns the names based on the entered data
            user.FirstName = returnedUser.FirstName;
            user.LastName = returnedUser.LastName;

            // Saves the user details into the database
            await userManager.UpdateAsync(user);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        //EndDocSection:EditUser


        //DocSection:UserAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangeAvatar(IFormFile avatarUpload)
        {
            object routevalues = null;

            // Checks that the avatar file was uploaded
            if (avatarUpload != null && avatarUpload.Length > 0)
            {
                // Gets the representation of the user requesting avatar change from Xperience
                ApplicationUser user = await userManager.FindByNameAsync(User.Identity.Name);
                // Attempts to update the user's avatar
                if (!avatarService.UpdateAvatar(avatarUpload.ToUploadedFile(), user.Id, siteService.CurrentSite.SiteName))
                {
                    // If the avatar update failed (e.g., the user uploaded an image with an usupported file extension)
                    // sets a flag for an error message in the corresponding view
                    routevalues = new { avatarUpdateFailed = true };
                }
            }

            return RedirectToAction(nameof(EditUser), routevalues);
        }
        //EndDocSection:UserAvatar


        //DocSection:DeleteAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAvatar(int userId)
        {
            // Deletes the avatar image associated with the user
            avatarService.DeleteAvatar(userId);

            return RedirectToAction(nameof(EditUser));
        }
        //EndDocSection:DeleteAvatar
    }
}