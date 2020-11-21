using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LearningKitCore.Components.FormBuilder
{
    public static class FormBuilderHelper
    {
        //DocSection:CustomInputExtensionMethod
        // Renders an 'input' element of the specified type and with the collection of provided attributes
        public static IHtmlContent CustomInput(this IHtmlHelper helper, string inputType, string name, object value, IDictionary<string, object> htmlAttributes)
        {
            TagBuilder tagBuilder = new TagBuilder("input");
            tagBuilder.TagRenderMode = TagRenderMode.StartTag;

            // Specifies the input type, name, and value attributes
            tagBuilder.MergeAttribute("type", inputType);
            tagBuilder.MergeAttribute("name", name);
            tagBuilder.MergeAttribute("value", value.ToString());

            // Merges additional attributes into the element
            tagBuilder.MergeAttributes(htmlAttributes);

            using (var writer = new StringWriter())
            {
                tagBuilder.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                return new HtmlString(writer.ToString());
            }            
        }
        //EndDocSection:CustomInputExtensionMethod
    }
}
