using System;

using Microsoft.AspNetCore.Mvc;

using CMS.Base;
using CMS.Newsletters;
using CMS.ContactManagement;
using CMS.Helpers;

using LearningKitCore.Models.OnlineMarketing;


namespace LearningKitCore.Controllers.OnlineMarketing
{
    public class NewsletterSubscriptionController : Controller
    {

        //DocSection:SubscriptionServices
        private readonly ISubscriptionService subscriptionService;
        private readonly IUnsubscriptionProvider unsubscriptionProvider;
        private readonly IContactProvider contactProvider;
        private readonly IEmailHashValidator emailHashValidator;
        private readonly ISubscriptionApprovalService subscriptionApprovalService;
        private readonly IIssueInfoProvider issueInfoProvider;
        private readonly INewsletterInfoProvider newsletterInfoProvider;
        private readonly ISiteService siteService;

        public NewsletterSubscriptionController(ISubscriptionService subscriptionService,
                                                IUnsubscriptionProvider unsubscriptionProvider,
                                                IContactProvider contactProvider,
                                                IEmailHashValidator emailHashValidator,
                                                ISubscriptionApprovalService subscriptionApprovalService,
                                                IIssueInfoProvider issueInfoProvider,
                                                INewsletterInfoProvider newsletterInfoProvider,
                                                ISiteService siteService)
        {
            this.subscriptionService = subscriptionService;
            this.unsubscriptionProvider = unsubscriptionProvider;
            this.contactProvider = contactProvider;
            this.emailHashValidator = emailHashValidator;
            this.subscriptionApprovalService = subscriptionApprovalService;
            this.issueInfoProvider = issueInfoProvider;
            this.newsletterInfoProvider = newsletterInfoProvider;
            this.siteService = siteService;
        }
        //EndDocSection:SubscriptionServices


        //DocSection:Subscribe
        /// <summary>
        /// Basic action that displays the newsletter subscription form.
        /// </summary>
        public IActionResult Subscribe()
        {
            return View();
        }


        /// <summary>
        /// Handles creation of new marketing email recipients when the subscription form is submitted.
        /// Accepts an email address parameter posted from the subscription form.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Subscribe(NewsletterSubscriptionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // If the subscription data is not valid, displays the subscription form with error messages
                return View(model);
            }

            // Either gets an existing contact by email or creates a new contact object with the given email
            ContactInfo contact = contactProvider.GetContactForSubscribing(model.Email);

            // Gets a newsletter
            // Fill in the code name of your newsletter object in Xperience
            NewsletterInfo newsletter = newsletterInfoProvider.Get("SampleNewsletter", siteService.CurrentSite.SiteID);

            // Prepares settings that configure the subscription behavior
            var subscriptionSettings = new SubscribeSettings()
            {
                // Subscription removes the given email address from the global opt-out list for all marketing emails (if present)
                RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                // Allows sending of confirmation emails for the subscription
                SendConfirmationEmail = true,
                // Allows handling of double opt-in subscription for newsletters that have it enabled
                AllowOptIn = true 
            };

            // Subscribes the contact with the specified email address to the given newsletter
            subscriptionService.Subscribe(contact, newsletter, subscriptionSettings);

            // Passes information to the view, indicating whether the newsletter requires double opt-in for subscription
            model.RequireDoubleOptIn = newsletter.NewsletterEnableOptIn;

            // Displays a view to inform the user that the subscription was successful
            return RedirectToAction(nameof(SubscribeSuccess), new { requireDoubleOptIn = model.RequireDoubleOptIn});
        }


        public IActionResult SubscribeSuccess(bool requireDoubleOptIn = false)
        {
            return View(requireDoubleOptIn);
        }
        //EndDocSection:Subscribe


        //DocSection:Unsubscribe
        /// <summary>
        /// Handles marketing email subscription cancellation requests.
        /// </summary>
        public IActionResult Unsubscribe(NewsletterUnsubscriptionViewModel model)
        {
            // Verifies that the subscription cancellation request contains all required parameters
            if (ModelState.IsValid)
            {
                // Confirms whether the hash in the subscription cancellation request is valid for the given email address
                // Provides protection against forged cancellation requests
                if (emailHashValidator.ValidateEmailHash(model.Hash, model.Email))
                {
                    // Gets the marketing email (issue) from which the cancellation request was sent
                    IssueInfo issue = issueInfoProvider.Get(model.IssueGuid, siteService.CurrentSite.SiteID);

                    if (model.UnsubscribeFromAll)
                    {
                        // Checks that the email address is not already unsubscribed from all marketing emails
                        if (!unsubscriptionProvider.IsUnsubscribedFromAllNewsletters(model.Email))
                        {
                            // Unsubscribes the specified email address from all marketing emails (adds it to the opt-out list)
                            subscriptionService.UnsubscribeFromAllNewsletters(model.Email, issue?.IssueID);
                        }
                    }
                    else
                    {
                        // Gets the email feed for which the cancellation was requested
                        NewsletterInfo newsletter = newsletterInfoProvider.Get(model.NewsletterGuid, siteService.CurrentSite.SiteID);

                        if (newsletter != null)
                        {
                            // Checks that the email address is not already unsubscribed from the specified email feed
                            if (!unsubscriptionProvider.IsUnsubscribedFromSingleNewsletter(model.Email, newsletter.NewsletterID))
                            {
                                // Unsubscribes the specified email address from the email feed
                                subscriptionService.UnsubscribeFromSingleNewsletter(model.Email, newsletter.NewsletterID, issue?.IssueID);
                            }
                        }
                    }

                    // Displays a view to inform the user that they were unsubscribed
                    return RedirectToAction(nameof(UnsubscribeSuccess));
                }
            }

            // If the subscription cancellation request was not successful, displays a view to inform the user
            // Failure can occur if the request does not provide all required parameters or if the hash is invalid
            return RedirectToAction(nameof(UnsubscribeFailure));
        }


        public IActionResult UnsubscribeSuccess()
        {
            return View();
        }


        public IActionResult UnsubscribeFailure()
        {
            return View();
        }
        //EndDocSection:Unsubscribe


        //DocSection:ConfirmSubscription
        /// <summary>
        /// Handles confirmation requests for newsletter subscriptions (when using double opt-in).
        /// </summary>
        public IActionResult ConfirmSubscription(NewsletterSubscriptionConfirmationViewModel model)
        {
            // Verifies that the confirmation request contains the required hash parameter
            if (!ModelState.IsValid)
            {
                // If the hash is missing, returns a view informing the user that the subscription confirmation was not successful
                ModelState.AddModelError(String.Empty, "The confirmation link is invalid.");
                return View(model);
            }

            // Attempts to parse the date and time parameter from the request query string
            // Uses the date and time formats required by the Xperience API
            DateTime parsedDateTime = DateTimeHelper.ZERO_TIME;
            if (!string.IsNullOrEmpty(model.DateTime) && !DateTimeUrlFormatter.TryParse(model.DateTime, out parsedDateTime))
            {
                // Returns a view informing the user that the subscription confirmation was not successful
                ModelState.AddModelError(String.Empty, "The confirmation link is invalid.");
                return View(model);
            }

            // Attempts to confirm the subscription specified by the request's parameters
            ApprovalResult result = subscriptionApprovalService.ApproveSubscription(model.SubscriptionHash, false, siteService.CurrentSite.SiteName, parsedDateTime);

            switch (result)
            {
                // The confirmation was successful or the recipient was already approved
                // Displays a view informing the user that the subscription is active
                case ApprovalResult.Success:
                case ApprovalResult.AlreadyApproved:
                    return View(model);

                // The confirmation link has expired
                // Expiration occurs after a certain number of hours from the time of the original subscription
                // You can set the expiration interval in Xperience (Settings -> On‑line marketing -> Email marketing -> Double opt-in interval)
                case ApprovalResult.TimeExceeded:
                    ModelState.AddModelError(String.Empty, "Your confirmation link has expired. Please subscribe to the newsletter again.");
                    break;

                // The subscription specified in the request's parameters does not exist
                case ApprovalResult.NotFound:
                    ModelState.AddModelError(String.Empty, "The subscription that you are attempting to confirm does not exist.");
                    break;

                // The confirmation failed
                default:
                    ModelState.AddModelError(String.Empty, "The confirmation of your newsletter subscription did not succeed.");
                    break;
            }

            // If the subscription confirmation was not successful, displays a view informing the user
            return View(model);
        }
        //EndDocSection:ConfirmSubscription
    }
}
