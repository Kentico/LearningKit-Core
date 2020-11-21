using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using CMS.Search;
using CMS.Membership;

using LearningKitCore.Models.Search.SmartSearch;


namespace LearningKitCore.Controllers.Search
{
    public class SmartSearchController : Controller
    {
        //DocSection:SmartySearchController
        private const string INDEX_NAME = "CoreSite.Index";

        // Sets the limit of items per page of search results
        private const int PAGE_SIZE = 10;

        // Adds the smart search indexes that will be used to perform searches
        public static readonly string[] searchIndexes = new string[] { INDEX_NAME };


        /// <summary>
        /// Performs a search and displays its result.
        /// </summary>
        public IActionResult SearchIndex(string searchText)
        {
            // Displays the search page without any search results if the query is empty
            if (String.IsNullOrWhiteSpace(searchText))
            {
                // Creates a model representing empty search results
                SearchResultModel emptyModel = new SearchResultModel
                {
                    Items = new List<SearchResultItem>(),
                    Query = String.Empty
                };

                return View(emptyModel);
            }

            // Searches the specified index and gets the matching results
            SearchParameters searchParameters = SearchParameters.PrepareForPages(searchText, searchIndexes, 1, PAGE_SIZE, MembershipContext.AuthenticatedUser, "en-us", true);
            SearchResult searchResult = SearchHelper.Search(searchParameters);

            // Creates a model with the search result items
            SearchResultModel model = new SearchResultModel
            {
                Items = searchResult.Items,
                Query = searchText
            };

            return View(model);
        }
        //EndDocSection:SmartySearchController
    }
}
