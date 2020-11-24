﻿using Kentico.Forms.Web.Mvc;

using LearningKitCore.Components.FormBuilder.FormComponents.CustomFormComponent;

//DocSection:FormComponentRegistration
// Registers a form component for use in the form builder
[assembly: RegisterFormComponent(CustomFormComponent.IDENTIFIER,
                                 typeof(CustomFormComponent),
                                 "Custom component",
                                 Description = "This is a custom form component.",
                                 IconClass = "icon-newspaper",
                                 ViewName = "~/Components/FormBuilder/FormComponents/CustomFormComponent/_CustomFormComponent.cshtml")]
//EndDocSection:FormComponentRegistration

namespace LearningKitCore.Components.FormBuilder.FormComponents.CustomFormComponent
{
    //DocSection:FormComponentImplementation
    public class CustomFormComponent : FormComponent<CustomFormComponentProperties, string>
    {
        public const string IDENTIFIER = "CustomFormComponent";


        // Specifies the property is used for data binding by the form builder
        [BindableProperty]
        // Used to store the value of the input field of the component
        public string Value { get; set; }


        // Gets the value of the form field instance passed from a view where the instance is rendered
        public override string GetValue()
        {
            return Value;
        }


        // Sets the default value of the form field instance
        public override void SetValue(string value)
        {
            Value = value;
        }
    }
    //EndDocSection:FormComponentImplementation
}