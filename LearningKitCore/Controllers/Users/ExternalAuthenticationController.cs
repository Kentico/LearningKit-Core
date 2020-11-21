//DocSection:Using
using System;
using System.Threading.Tasks;
using System.Security.Claims;

using CMS.Helpers;
using CMS.Core;
using CMS.Base;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

using Kentico.Membership;
//EndDocSection:Using

namespace LearningKit.Controllers
{
    public class ExternalAuthenticationController : Controller
    {
        //DocSection:DependencyInjection
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ApplicationUserManager<ApplicationUser> userManager;
        private readonly IEventLogService eventLogService;
        private readonly ISiteService siteService;

        public ExternalAuthenticationController(SignInManager<ApplicationUser> signInManager,
                                                ApplicationUserManager<ApplicationUser> userManager,
                                                IEventLogService eventLogService,
                                                ISiteService siteService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.eventLogService = eventLogService;
            this.siteService = siteService;
        }
        //EndDocSection:DependencyInjection


        //DocSection:ExternalAuth
        /// <summary>
        /// Redirects authentication requests to an external service. 
        /// Posted parameters include the name of the requested authentication middleware instance and a return URL.
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult RequestExternalSignIn(string provider, string returnUrl)
        {
            // Sets a return URL targeting an action that handles the response
            string redirectUrl = Url.Action(nameof(ExternalSignInCallback), new { ReturnUrl = returnUrl });

            // Configures the redirect URL and user identifier for the specified external authentication provider
            AuthenticationProperties authenticationProperties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            // Challenges the specified authentiction provider, redirects to the specified 'redirectURL' when done
            return Challenge(authenticationProperties, provider);
        }


        /// <summary>
        /// Handles responses from external authentication services.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalSignInCallback(string returnUrl, string remoteError = null)
        {
            // If an error occurred on the side of the external provider, displays a view with the forwarded error message
            if (remoteError != null)
            {
                return RedirectToAction(nameof(ExternalAuthenticationFailure));
            }

            // Extracts login info out of the external identity provided by the service
            ExternalLoginInfo loginInfo = await signInManager.GetExternalLoginInfoAsync();

            // If the external authentication fails, displays a view with appropriate information
            if (loginInfo == null)
            {
                return RedirectToAction(nameof(ExternalAuthenticationFailure));
            }

            // Attempts to sign in the user using the external login info
            SignInResult result = await signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false);

            // Success occurs if the user already exists in the connected database and has signed in using the given external service
            if (result.Succeeded)
            {
                eventLogService.LogInformation("External authentication", "EXTERNALAUTH", $"User logged in with {loginInfo.LoginProvider} provider.");
                return RedirectToLocal(returnUrl);
            }

            // The 'NotAllowed' status occurs if the user exists in the system, but is not enabled
            if (result.IsNotAllowed)
            {
                // Returns a view informing the user about the locked account
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // Attempts to create a new user in Xperience if the authentication failed
                IdentityResult userCreation = await CreateExternalUser(loginInfo);

                // Attempts to sign in again with the new user created based on the external authentication data
                result = await signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false);

                // Verifies that the user was created successfully and was able to sign in
                if (userCreation.Succeeded && result == SignInResult.Success)
                {
                    // Redirects to the original return URL
                    return RedirectToLocal(returnUrl);
                }

                // If the user creation was not successful, displays corresponding error messages
                foreach (IdentityError error in userCreation.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }

                return View();

                // Optional extension:
                // Send the loginInfo data as a view model and create a form that allows adjustments of the user data.
                // Allows visitors to resolve errors such as conflicts with existing usernames in Xperience.
                // Then post the data to another action and attempt to create the user account again.
                // The action can access the original loginInfo using the AuthenticationManager.GetExternalLoginInfoAsync() method.
            }           
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalAuthenticationFailure()
        {
            return View();
        }


        /// <summary>
        /// Creates users in Xperience based on external login data.
        /// </summary>
        private async Task<IdentityResult> CreateExternalUser(ExternalLoginInfo loginInfo)
        {
            // Prepares a new user entity based on the external login data
            ApplicationUser user = new ApplicationUser
            {
                UserName = ValidationHelper.GetSafeUserName(loginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? 
                                                            loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                                                            siteService.CurrentSite.SiteName),
                Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email),
                Enabled = true, // The user is enabled by default
                IsExternal = true // IsExternal must always be true for users created via external authentication
                // Set any other required user properties using the data available in loginInfo
            };

            // Attempts to create the user in the Xperience database
            IdentityResult result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                // If the user was created successfully, creates a mapping between the user and the given authentication provider
                result = await userManager.AddLoginAsync(user, loginInfo);
            }

            return result;
        }


        /// <summary>
        /// Redirects to a specified return URL if it belongs to the MVC website or to the site's home page.
        /// </summary>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(nameof(Index), "Home");
        }
        //EndDocSection:ExternalAuth
    }
}