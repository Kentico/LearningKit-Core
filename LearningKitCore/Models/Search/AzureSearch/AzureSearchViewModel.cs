using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningKitCore.Models.Search.AzureSearch
{
    public class AzureSearchViewModel
    {
        public string SearchString { get; set; }

        public IList<DocumentViewModel> SearchResults { get; set; }

        public IList<FacetViewModel> FilterFarm { get; set; }

        public IList<FacetViewModel> FilterCountry { get; set; }

        public AzureSearchViewModel()
        {
            FilterCountry = new List<FacetViewModel>();
            FilterFarm = new List<FacetViewModel>();
            SearchResults = new List<DocumentViewModel>();
        }
    }
}
