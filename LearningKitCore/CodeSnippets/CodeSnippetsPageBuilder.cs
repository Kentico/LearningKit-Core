using System.Linq;

using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Kentico.Web.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace LearningKit.Areas.CodeSnippets
{
    public class PageBuilderInitializationExample : Controller
    {
        //DocSection:PageBuilderInitialize
        private readonly IPageRetriever pagesRetriever;
        private readonly IPageDataContextInitializer pageDataContextInitializer;

        // Gets instances of required services using dependency injection
        public PageBuilderInitializationExample(IPageRetriever pagesRetriever,
                                                IPageDataContextInitializer pageDataContextInitializer)
        {
            this.pagesRetriever = pagesRetriever;
            this.pageDataContextInitializer = pageDataContextInitializer;
        }

        public ActionResult Home()
        {
            // Retrieves a page from the Xperience database with the '/Home' node alias path
            TreeNode page = pagesRetriever.Retrieve<TreeNode>(query => query
                                .Path("/Home", PathTypeEnum.Single))
                                .FirstOrDefault();

            // Responds with the HTTP 404 error when the page is not found
            if (page == null)
            {
                return NotFound();
            }

            // Initializes the page data context (and the page builder) using the retrieved page
            pageDataContextInitializer.Initialize(page);

            return View();
        }
        //EndDocSection:PageBuilderInitialize

        //DocSection:AssignDefaultSection
        public void PageBuilderAssignDefaultSection(IServiceCollection services)
        {
            PageBuilderOptions options = new PageBuilderOptions()
            {
                DefaultSectionIdentifier = "CompanyName.DefaultSection",
                // Disables the system's built-in 'Default' section
                RegisterDefaultSection = false
            };

            services.AddKentico(features =>
                features.UsePageBuilder(options));
        }
        //EndDocSection:AssignDefaultSection
    }

    public class PageTemplateCustomRoutingInitialization : Controller
    {
        //DocSection:PageTemplateAction
        private readonly IPageRetriever pagesRetriever;

        // Gets instances of required services using dependency injection
        public PageTemplateCustomRoutingInitialization(IPageRetriever pagesRetriever)
        {
            this.pagesRetriever = pagesRetriever;
        }

        /// <summary>
        /// A GET action displaying a page where you wish to use page templates.
        /// </summary>
        /// <param name="pageAlias">Page alias of the displayed page.</param>
        public ActionResult Index(string pageAlias)
        {
            // Retrieves a page from the Xperience database
            TreeNode page = pagesRetriever.Retrieve<TreeNode>(query => query
                                .Path("/Landing-pages", PathTypeEnum.Children)
                                .WhereEquals("NodeAlias", pageAlias)
                                .TopN(1))
                                .FirstOrDefault();

            // Responds with the HTTP 404 error when the page is not found
            if (page == null)
            {
                return NotFound();
            }

            // Returns a TemplateResult object, created with the retrieved page
            // Automatically initializes the page data context and the page builder feature
            // for all editable areas placed within templates
            return new TemplateResult(page);
        }
        //EndDocSection:PageTemplateAction
    }

    public class PageTemplateAdvancedRoutingInitialization : Controller
    {
        //DocSection:PageTemplatesAdvancedRouting
        public ActionResult Index()
        {
            // Custom processing logic

            // Leverages information provided by the router when serving the request
            // to retrieve the corresponding page. No need to specify the page to render.
            return new TemplateResult();
        }
        //EndDocSection:PageTemplatesAdvancedRouting
    }
}