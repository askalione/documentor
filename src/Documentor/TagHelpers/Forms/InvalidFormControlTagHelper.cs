using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Structr.AspNetCore.TagHelpers;

namespace Documentor.TagHelpers.Forms
{
    [HtmlTargetElement("textarea", Attributes = "asp-for")]
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class InvalidFormControlTagHelper : TagHelper
    {
        private const string _defaultClassValue = "form-control";
        private const string _invalidClassValue = "is-invalid";

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; } = default!;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.AddClass(_defaultClassValue);

            var modelState = ViewContext.ModelState;
            if (modelState.TryGetValue(For.Name, out ModelStateEntry? propertyState)
                && propertyState != null && propertyState.Errors.Count > 0)
            {
                output.AddClass(_invalidClassValue);
            }
        }
    }
}
