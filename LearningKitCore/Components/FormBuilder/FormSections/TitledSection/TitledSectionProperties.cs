using Kentico.Forms.Web.Mvc;

namespace LearningKitCore.Components.FormBuilder.FormSections.TitledSection
{
    public class TitledSectionProperties : IFormSectionProperties
    {
        // Defines a title property whose value is edited via a text input in the section configuration dialog
        [EditingComponent(TextInputComponent.IDENTIFIER, Order = 0, Label = "Title")]
        public string Title { get; set; } = "General";
    }
}
