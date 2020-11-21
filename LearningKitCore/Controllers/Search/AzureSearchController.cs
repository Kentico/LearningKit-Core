using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

using CMS.Search;
using CMS.Search.Azure;

using LearningKitCore.Models.Search.AzureSearch;


namespace LearningKitCore.Controllers.Search
{
    public class AzureSearchController : Controller
    {
        private const string INDEX_NAME = "dg-store";

        private ISearchIndexClient searchIndexClient = InitializeIndex(INDEX_NAME);

        // The fields used for faceted navigation
        private const string FACET_COFFEE_COUNTRY = "coffeecountry";

        private const string FACET_COFFEE_FARM = "coffeefarm";

        public readonly List<string> Facets = new List<string>
        {
            FACET_COFFEE_COUNTRY,
            FACET_COFFEE_FARM
        };


        // Returns an initialized 'SearchServiceClient' instance for the specified index
        private static ISearchIndexClient InitializeIndex(string indexCodeName)
        {
            // Converts the Xperience index code name to a valid Azure Search index name (if necessary)
            indexCodeName = NamingHelper.GetValidIndexName(indexCodeName);

            SearchIndexInfo index = SearchIndexInfoProvider.GetSearchIndexInfo(indexCodeName);
            SearchServiceClient client = new SearchServiceClient(index.IndexSearchServiceName, new SearchCredentials(index.IndexAdminKey));

            return client.Indexes.GetClient(indexCodeName);
        }


        // Displays a search interface, listing search results from the entire index
        public IActionResult Index()
        {
            // Prepares a list of filter queries for the search request
            IList<string> filterQueries = InitializeFilterQueries();

            // Configures the 'SearchParameters' object
            Microsoft.Azure.Search.Models.SearchParameters searchParams = ConfigureSearchParameters(filterQueries);

            // Returns all results on initial page load
            string searchString = "*";

            // Prepares a view model used to hold the search results and search configuration
            AzureSearchViewModel model = new AzureSearchViewModel();

            // Performs a search request on the specified Azure Search index with the configured 'SearchParameters' object
            DocumentSearchResult<Document> result = searchIndexClient.Documents.Search(searchString, searchParams);

            // Fills the corresponding view model with facets retrieved from the search query
            if (result.Facets != null)
            {
                foreach (var facet in result.Facets.Take(5))
                {
                    foreach (FacetResult value in facet.Value)
                    {
                        AddFacet(model, value, facet.Key);
                    }
                }
            }

            // Fills the view model with search results and displays them in a corresponding view
            return View("SearchIndex", PrepareSearchResultsViewModel(result, model));
        }


        // Processes the submitted search request and displays a list of results
        [HttpPost]
        public IActionResult Search(AzureSearchViewModel searchSettings)
        {
            // Prepares a list of filter queries for the search request
            IList<string> filterQueries = InitializeFilterQueries();

            // Adds filter queries based on the selected options in the faceted navigation (coffee farm and region)
            IEnumerable<FacetViewModel> selectedCountries = searchSettings.FilterCountry?.Where(x => x.Selected);
            IEnumerable<FacetViewModel> selectedFarms = searchSettings.FilterFarm?.Where(x => x.Selected);

            if (selectedCountries.Any())
            {
                filterQueries.Add(GetFilterQuery(selectedCountries, FACET_COFFEE_COUNTRY));
            }
            if (selectedFarms.Any())
            {
                filterQueries.Add(GetFilterQuery(selectedFarms, FACET_COFFEE_FARM));
            }

            // Prepares the search parameters
            Microsoft.Azure.Search.Models.SearchParameters searchParams = ConfigureSearchParameters(filterQueries);

            // Gets the search text from the input
            string searchString = searchSettings.SearchString;

            // Performs the search request for the specified Azure Search index and parameters
            DocumentSearchResult<Document> result = searchIndexClient.Documents.Search(searchString, searchParams);

            // Fills or updates the faceted navigation options
            if (result.Facets != null)
            {
                foreach (FacetViewModel item in searchSettings.FilterCountry)
                {
                    UpdateFacets(result.Facets[FACET_COFFEE_COUNTRY], item);
                }

                foreach (FacetViewModel item in searchSettings.FilterFarm)
                {
                    UpdateFacets(result.Facets[FACET_COFFEE_FARM], item);
                }
            }

            // Displays the search results
            return View("SearchIndex", PrepareSearchResultsViewModel(result, searchSettings));
        }


        // Initializes a list of filter queries
        private static List<string> InitializeFilterQueries()
        {
            var filterQueries = new List<string>()
            {
                // Filters the search results to display only pages (products) of the 'dancinggoatcore.coffee' type
                "classname eq 'dancinggoatcore.coffee'"
            };

            return filterQueries;
        }


        // Configures the 'SearchParameters' object
        private Microsoft.Azure.Search.Models.SearchParameters ConfigureSearchParameters(IList<string> filterQueries)
        {
            var searchParams = new Microsoft.Azure.Search.Models.SearchParameters
            {
                Facets = Facets,
                Filter = String.Join(" and ", filterQueries),
                HighlightPreTag = "<strong>",
                HighlightPostTag = "</strong>",
                // All fields used for text highlighting must be configured as 'searchable'
                HighlightFields = new List<string>
                {
                    FACET_COFFEE_COUNTRY,
                    FACET_COFFEE_FARM,
                    "skushortdescription",
                    "skuname"
                }
            };

            return searchParams;
        }


        // Builds a filter query based on selected faceted navigation options
        private string GetFilterQuery(IEnumerable<FacetViewModel> selectedItems, string column)
        {
            var queries = selectedItems.Select(item => $"{column} eq '{item.Value.Replace("'", "''")}'");
            return String.Join(" or ", queries);
        }


        // Adds a retrieved 'FacetResult' to the list of facets in the corresponding model
        private void AddFacet(AzureSearchViewModel model, FacetResult facetResult, string resultFacetKey)
        {
            FacetViewModel item = new FacetViewModel() { Name = $"{facetResult.Value} ({facetResult.Count})", Value = facetResult.Value.ToString() };
            switch (resultFacetKey)
            {
                case FACET_COFFEE_COUNTRY:
                    model.FilterCountry.Add(item);
                    break;
                case FACET_COFFEE_FARM:
                    model.FilterFarm.Add(item);
                    break;
                default:
                    break;
            }
        }


        // Updates the counts of matching results for the processed query
        private static void UpdateFacets(IEnumerable<FacetResult> facetResults, FacetViewModel listItem)
        {
            long? count = 0;
            foreach (var items in facetResults)
            {
                if (items.Value.Equals(listItem.Value))
                {
                    count = items.Count;
                    break;
                }
            }

            listItem.Name = $"{listItem.Value} ({count})";
        }


        // Fills a view model with retrieved search results and faceted navigation options
        private AzureSearchViewModel PrepareSearchResultsViewModel(DocumentSearchResult<Document> searchResult, AzureSearchViewModel model)
        {
            if (searchResult.Results.Count == 0)
            {
                model.SearchResults = null;
                return model;
            }

            foreach (SearchResult<Document> result in searchResult.Results)
            {
                model.SearchResults.Add(new DocumentViewModel()
                {
                    DocumentTitle = $"{result.Document["skuname"]}",
                    DocumentShortDescription = $"{result.Document["skushortdescription"]}",
                    Highlights = result.Highlights
                });
            }

            return model;
        }
    }
}
