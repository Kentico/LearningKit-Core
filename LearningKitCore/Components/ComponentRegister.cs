using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using LearningKitCore.Components.PageBuilder.Widgets.ColorWidget;
using LearningKitCore.Components.PageBuilder.Widgets.NumberWidget;
using LearningKitCore.Components.FormBuilder.FormSections.TitledSection;

// ================================= Form builder registrations ====================================

[assembly: RegisterFormSection("LearningKit.FormSections.CustomSection", 
                               "Custom section",
                               customViewName: "~/Components/FormBuilder/FormSections/CustomFormSection.cshtml")]

[assembly: RegisterFormSection("LearningKit.FormSections.TitledSection", 
                               "Titled section",
                               customViewName: "~/Components/FormBuilder/FormSections/TitledSection/TitledSection.cshtml",
                               PropertiesType = typeof(TitledSectionProperties))]

// ================================= Page builder registrations ====================================

//DocSection:ExampleSection
[assembly: RegisterSection("MyCompany.Sections.CustomSection",
                          "Custom section",
                          typeof(CustomSectionProperties),
                          customViewName: "~/Components/PageBuilder/Sections/CustomSection/_CustomSection.cshtml",
                          IconClass = "icon-square")]
//EndDocSection:ExampleSection

//DocSection:ExampleWidget
[assembly: RegisterWidget("LearningKit.Widgets.NumberWidget",
                         "Number selector",
                         typeof(NumberWidgetProperties),
                         customViewName: "~/Components/PageBuilder/Widgets/NumberWidget/_NumberWidget.cshtml")]
//EndDocSection:ExampleWidget

[assembly: RegisterWidget("LearningKit.Widgets.ColorWidget",
                            "Colored widget", 
                            typeof(ColorWidgetProperties), 
                            customViewName: "~/Components/PageBuilder/Widgets/ColorWidget/_ColorWidget.cshtml", 
                            IconClass = "icon-palette")]

// Registers the default page builder section used by the LearningKit project
[assembly: RegisterSection("LearningKit.Sections.DefaultSection",
                           "Default section", 
                           customViewName: "~/Components/PageBuilder/Sections/_DefaultSection.cshtml", 
                           IconClass = "icon-square")]
// Registers a two-column section that splits the page down the middle
[assembly: RegisterSection("LearningKit.Sections.Col5050",
                           "50/50 columns", 
                           customViewName: "~/Components/PageBuilder/Sections/_Col5050.cshtml")]

// Registers the 'Extended form' widget
[assembly: RegisterWidget("LearningKit.Widgets.ExtendedFormWidget",
                          "Extended form",
                          typeof(ExtendedFormWidgetProperties),
                          customViewName: "~/Components/PageBuilder/Widgets/ExtendedFormWidget/_ExtendedFormWidget.cshtml",
                          IconClass = "icon-form")]
