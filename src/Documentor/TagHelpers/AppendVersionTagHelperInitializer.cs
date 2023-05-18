using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Documentor.TagHelpers
{
    public class AppendVersionTagHelperInitializer : ITagHelperInitializer<ScriptTagHelper>, ITagHelperInitializer<LinkTagHelper>
    {
        private const bool DefaultValue = true;

        public void Initialize(ScriptTagHelper helper, ViewContext context)
        {
            helper.AppendVersion = DefaultValue;
        }

        public void Initialize(LinkTagHelper helper, ViewContext context)
        {
            helper.AppendVersion = DefaultValue;
        }
    }
}
