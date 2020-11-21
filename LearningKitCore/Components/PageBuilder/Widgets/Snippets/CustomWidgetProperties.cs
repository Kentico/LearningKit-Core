using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;


namespace LearningKitCore.Components.Widgets
{
    //DocSection:WidgetPropertyClass
    public class CustomWidgetProperties : IWidgetProperties
    {        
        // Defines a property and sets its default value
        public int Number { get; set; } = 22;
    }
    //EndDocSection:WidgetPropertyClass

    public class WidgetPropertiesSnippets
    {
        //DocSection:PropertyConfigurationDialog
        [EditingComponent(TextInputComponent.IDENTIFIER, Order = 0, Label = "Text")]
        public string Text { get; set; }
        //EndDocSection:PropertyConfigurationDialog
    }
}
