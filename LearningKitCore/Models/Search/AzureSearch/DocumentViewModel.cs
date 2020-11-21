using System.Collections.Generic;

namespace LearningKitCore.Models.Search.AzureSearch
{
    public class DocumentViewModel
    {
        public string DocumentTitle { get; set; }

        public string DocumentShortDescription { get; set; }

        public IDictionary<string, IList<string>> Highlights { get; set; }
    }
}
