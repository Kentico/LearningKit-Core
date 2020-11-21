using System.Collections.Generic;
using System.Linq;

using Kentico.Forms.Web.Mvc;
using Kentico.Forms.Web.Mvc.FormComponents;

namespace LearningKitCore.Components.FormBuilder.FormComponentFilters
{
    public class FormComponentsFilter
    {
        public IEnumerable<FormComponentDefinition> Filter(IEnumerable<FormComponentDefinition> formComponents, FormComponentFilterContext context)
        {
            // Filters out all Xperience form components from the form builder UI
            return formComponents.Where(component => !component.Identifier.StartsWith("Kentico"));
        }
    }
}
