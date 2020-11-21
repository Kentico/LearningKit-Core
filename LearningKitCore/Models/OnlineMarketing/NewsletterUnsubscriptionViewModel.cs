using System;

namespace LearningKitCore.Models.OnlineMarketing
{
    public class NewsletterUnsubscriptionViewModel
    {
        public string Hash { get; set; }

        public string Email { get; set; }

        public Guid IssueGuid { get; set; }

        public Guid NewsletterGuid { get; set; }

        public bool UnsubscribeFromAll { get; set; }
    }
}
