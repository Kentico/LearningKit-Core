using System.Collections.Generic;

using CMS.DataProtection;


namespace LearningKitCore.Models.DataProtection.PrivacyPage
{
    public class ConsentListingModel
    {
        public IEnumerable<Consent> Consents { get; set; }
    }
}
