using Kentico.Forms.Web.Mvc;
using LearningKitCore.Components.FormBuilder.FormComponents.ColorFormComponent;

[assembly: RegisterFormComponent(ColorFormComponent.IDENTIFIER,
                                 typeof(ColorFormComponent),
                                 "Color component",
                                 IsAvailableInFormBuilderEditor = false,
                                 IconClass = "icon-newspaper",
                                 ViewName = "~/Components/FormBuilder/FormComponents/ColorFormComponent/_ColorFormComponent.cshtml")]

namespace LearningKitCore.Components.FormBuilder.FormComponents.ColorFormComponent
{
    public class ColorFormComponent : FormComponent<ColorFormComponentProperties, string>
    {
        public const string IDENTIFIER = "ColorFormComponent";

        [BindableProperty]
        public string Value { get; set; }

        public override string GetValue()
        {
            return Value;
        }

        public override void SetValue(string value)
        {
            Value = value;
        }
    }
}