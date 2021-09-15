using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Forms.Web.Mvc;
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

    //DocSection:PathSelectorViewComponent
    public class PathSelectorWidget : ViewComponent
    {
        private readonly IPageRetriever pagesRetriever;

        public PathSelectorWidget(IPageRetriever pagesRetriever)
        {
            this.pagesRetriever = pagesRetriever;
        }

        public IViewComponentResult Invoke(ComponentViewModel<CustomWidgetProperties> properties)
        {            
            // Retrieves the node alias paths of the selected pages from the 'PagePaths' property
            string[] selectedPagePaths = properties?.Properties?.PagePaths?
                                                                .Select(i => i.NodeAliasPath)
                                                                .ToArray();

            // Retrieves the pages that correspond to the selected alias paths
            List<TreeNode> pages = pagesRetriever.Retrieve<TreeNode>(query => query
                                                 .Path(selectedPagePaths))
                                                 .ToList();

            // Custom logic...

            return View("~/Components/Widgets/PathSelectorWidget/_PathSelectorWidget.cshtml");
        }
    }
    //EndDocSection:PathSelectorViewComponent

    //DocSection:PageSelectorViewComponent
    public class PageSelectorWidget : ViewComponent
    {
        private readonly IPageRetriever pagesRetriever;

        public PageSelectorWidget(IPageRetriever pagesRetriever)
        {
            this.pagesRetriever = pagesRetriever;
        }

        public IViewComponentResult Invoke(ComponentViewModel<CustomWidgetProperties> properties)
        {
            // Retrieves the node GUIDs of the selected pages from the 'Pages' property
            List<Guid> selectedPageGuids = properties?.Properties?.Pages?
                                                                  .Select(i => i.NodeGuid)
                                                                  .ToList();

            // Retrieves the pages that correspond to the selected GUIDs
            List<TreeNode> pages = pagesRetriever.Retrieve<TreeNode>(query => query
                                                 .WhereIn("NodeGUID", selectedPageGuids))
                                                 .ToList();

            // Custom logic...

            return View("~/Components/Widgets/PageSelectorWidget/_PageSelectorWidget.cshtml");
        }            
    }
    //EndDocSection:PageSelectorViewComponent

    public class CustomWidgetProperties : IWidgetProperties
    {
        // Assigns a selector component to the Pages property
        [EditingComponent(PageSelector.IDENTIFIER)]
        // Limits the selection of pages to a subtree rooted at the 'Products' page
        [EditingComponentProperty(nameof(PageSelectorProperties.RootPath), "/Products")]
        // Sets an unlimited number of selectable pages
        [EditingComponentProperty(nameof(PageSelectorProperties.MaxPagesLimit), 0)]
        // Returns a list of page selector items (node GUIDs)
        public IEnumerable<PageSelectorItem> Pages { get; set; } = Enumerable.Empty<PageSelectorItem>();

        // Assigns a selector component to the 'PagePaths' property
        [EditingComponent(PathSelector.IDENTIFIER)]
        // Limits the selection of pages to a subtree rooted at the 'Products' page
        [EditingComponentProperty(nameof(PathSelectorProperties.RootPath), "/Products")]
        // Sets the maximum number of selected pages to 6
        [EditingComponentProperty(nameof(PathSelectorProperties.MaxPagesLimit), 6)]
        // Returns a list of path selector items (page paths)
        public IEnumerable<PathSelectorItem> PagePaths { get; set; } = Enumerable.Empty<PathSelectorItem>();
    }
}