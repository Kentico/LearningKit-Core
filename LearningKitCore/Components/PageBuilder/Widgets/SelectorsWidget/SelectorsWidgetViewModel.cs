using System;

namespace LearningKitCore.Components.PageBuilder.Widgets.SelectorsWidget
{
    public class SelectorsWidgetViewModel
    {
        public string MediaFileUrl { get; set; }

        public string DocumentPath { get; set; }

        public Guid? DocumentGuid { get; set; }

        public string AttachmentUrl { get; set; }
    }
}