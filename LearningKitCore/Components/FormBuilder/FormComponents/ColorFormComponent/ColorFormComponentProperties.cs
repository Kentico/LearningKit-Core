using CMS.DataEngine;

using Kentico.Forms.Web.Mvc;

namespace LearningKitCore.Components.FormBuilder.FormComponents.ColorFormComponent
{
    public class ColorFormComponentProperties : FormComponentProperties<string>
    {
        public ColorFormComponentProperties()
            : base(FieldDataType.Text, size:200)
        {
        }

        [DefaultValueEditingComponent(TextInputComponent.IDENTIFIER)]
        public override string DefaultValue {
            get;
            set;
        }

        
    }
}