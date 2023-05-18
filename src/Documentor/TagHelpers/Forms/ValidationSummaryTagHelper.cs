using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Documentor.TagHelpers.Forms
{
    [HtmlTargetElement("validation-summary", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ValidationSummaryTagHelper : TagHelper
    {
        [HtmlAttributeName("include-property-errors")]
        public bool IncludePropertyErrors { get; set; } = false;

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; } = default!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ModelStateDictionary modelState = ViewContext.ModelState;
            List<ModelError> errors = modelState.Where(x => IncludePropertyErrors
                ? true
                : x.Key.Equals(string.Empty)).SelectMany(keyValuePair => keyValuePair.Value!.Errors).ToList();

            if (errors.Count > 0)
            {
                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Attributes.Add("class", "alert alert-styled alert-danger mg-b-20");
                output.Content.AppendHtml("<div class=\"alert-icon\"><i class=\"la la-ban\"></i><span></span></div>");

                var wrapper = new TagBuilder("div");
                wrapper.AddCssClass("alert-body");

                var list = new TagBuilder("ul");
                list.AddCssClass("mg-0 pd-l-10");
                if (errors.Count == 1)
                {
                    list.AddCssClass("list-unstyled");
                }

                foreach (var error in errors)
                {
                    var item = new TagBuilder("li");
                    item.InnerHtml.AppendHtml(error.ErrorMessage.Replace(Environment.NewLine, "<br />"));
                    list.InnerHtml.AppendHtml(item);
                }

                wrapper.InnerHtml.AppendHtml(list);
                output.Content.AppendHtml(wrapper);
            }
            else
            {
                output.SuppressOutput();
            }
        }
    }
}
