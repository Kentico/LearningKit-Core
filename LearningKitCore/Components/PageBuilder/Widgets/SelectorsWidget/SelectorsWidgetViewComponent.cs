using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using CMS.Base;
using CMS.DocumentEngine;
using CMS.MediaLibrary;

using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using LearningKitCore.Components.PageBuilder.Widgets.SelectorsWidget;


// Assembly attribute to register the widget for the connected Xperience instance.
[assembly: RegisterWidget("LearningKit.Widgets.SelectorsWidget", 
                            typeof(SelectorsWidgetViewComponent),
                            "Selectors demo",
                            typeof(SelectorsWidgetProperties))]

namespace LearningKitCore.Components.PageBuilder.Widgets.SelectorsWidget
{
    public class SelectorsWidgetViewComponent : ViewComponent
    {
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;
        private readonly IMediaFileInfoProvider mediaFileInfoProvider;
        private readonly IMediaFileUrlRetriever mediaFileUrlRetriever;
        private readonly ISiteService siteService;

        public SelectorsWidgetViewComponent(IPageAttachmentUrlRetriever attachmentUrlRetriever,
                                         IMediaFileInfoProvider mediaFileInfoProvider,
                                         IMediaFileUrlRetriever mediaFileUrlRetriever,
                                         ISiteService siteService)
        {
            this.attachmentUrlRetriever = attachmentUrlRetriever;
            this.mediaFileInfoProvider = mediaFileInfoProvider;
            this.mediaFileUrlRetriever = mediaFileUrlRetriever;
            this.siteService = siteService;
        }

        public async Task<IViewComponentResult> InvokeAsync(ComponentViewModel<SelectorsWidgetProperties> properties)
        {
            string mediaFileUrl = await GetMediaFileUrl(properties?.Properties?.Images);            

            // Retrieves the Path and Guid values of the selected page
            string documentPath = properties?.Properties?.PagePaths?.FirstOrDefault()?.NodeAliasPath;
            Guid? documentGuid = properties?.Properties?.Pages?.FirstOrDefault()?.NodeGuid;

            string attachmentUrl = GetAttachmentUrl(properties?.Properties?.Attachments);      

            return View("~/Components/PageBuilder/Widgets/SelectorsWidget/_SelectorsWidget.cshtml",
                        new SelectorsWidgetViewModel
                        {
                            MediaFileUrl = mediaFileUrl,
                            DocumentGuid = documentGuid,
                            DocumentPath = documentPath,
                            AttachmentUrl = attachmentUrl
                        });     
        }

        // Returns the relative path to the first attachment selected via the page attachment selector component
        private string GetAttachmentUrl(IEnumerable<AttachmentSelectorItem> attachments)
        {
            Guid attachmentGuid = attachments?.FirstOrDefault()?.FileGuid ?? Guid.Empty;

            DocumentAttachment attachment = DocumentHelper.GetAttachment(attachmentGuid, siteService.CurrentSite.SiteName);

            if (attachment == null)
            {
                return null;
            }

            return attachmentUrlRetriever.Retrieve(attachment).RelativePath;
        }


        // Returns the relative path to the first image selected via the image selector component
        private async Task<string> GetMediaFileUrl(IEnumerable<MediaFilesSelectorItem> images)
        {
            // Retrieves GUID of the first selected media file from the 'Images' property
            Guid guid = images.FirstOrDefault()?.FileGuid ?? Guid.Empty;

            // Retrieves the MediaFileInfo object that corresponds to the selected media file GUID
            MediaFileInfo mediaFile = await mediaFileInfoProvider.GetAsync(guid, siteService.CurrentSite.SiteID);

            if (mediaFile == null)
            {
                return null;
            }

            return mediaFileUrlRetriever.Retrieve(mediaFile).RelativePath;
        }
    }
}