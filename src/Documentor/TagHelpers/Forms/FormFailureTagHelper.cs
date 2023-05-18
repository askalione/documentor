using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Structr.AspNetCore.TagHelpers;

namespace Documentor.TagHelpers.Forms
{
    [HtmlTargetElement("span", Attributes = "asp-validation-for")]
    public class FormFailureTagHelper : TagHelper
    {
        private const string _defaultClassValue = "form-control-failure";

        [HtmlAttributeName("asp-validation-for")]
        public ModelExpression For { get; set; } = default!;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.AddClass(_defaultClassValue);
        }
    }
}
