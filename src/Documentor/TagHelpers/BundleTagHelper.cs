using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.TagHelpers
{
    [HtmlTargetElement("bundle")]
    public class BundleTagHelper : TagHelper
    {
        public string Name { get; set; }

        private string _relativeName;
        private string _absoluteName;
        private readonly IHostingEnvironment _hostingEnvironment;

        public BundleTagHelper(IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            _hostingEnvironment = hostingEnvironment;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (String.IsNullOrWhiteSpace(Name))
            {
                output.SuppressOutput();
                return;
            }
            
            _relativeName = Name.Replace("wwwroot", "~");
            _absoluteName = Name.Replace("~", "wwwroot");

            if (_hostingEnvironment.IsDevelopment())
            {
                Bundle bundle = await GetBundleAsync();
                if (bundle != null)
                {
                    var outputFiles = Name.EndsWith(".js") ?
                        bundle.InputFiles.Select(inputFile => $"<script src='{inputFile.Replace("wwwroot", "")}' type='text/javascript'></script>") :
                        bundle.InputFiles.Select(inputFile => $"<link rel='stylesheet' href='{inputFile.Replace("wwwroot", "")}' />");

                    output.Content.AppendHtml(String.Join("\n", outputFiles));
                }
                else
                {
                    output.SuppressOutput();
                }
            }
            else
            {
                output.Content.AppendHtml(Name.EndsWith(".js") ? 
                    $"<script src='/{_relativeName.Replace("~", "")}' type='text/javascript'></script>" :
                    $"<link rel='stylesheet' href='{_relativeName.Replace("~", "")}' />");
            }
        }

        private async Task<Bundle> GetBundleAsync()
        {
            string configFile = Path.Combine(_hostingEnvironment.ContentRootPath, "bundleconfig.json");

            var file = new FileInfo(configFile);
            if (!file.Exists)
                return null;

            var bundles = JsonConvert.DeserializeObject<IEnumerable<Bundle>>(await File.ReadAllTextAsync(configFile));
            return (from b in bundles
                    where b.OutputFileName.EndsWith(_absoluteName, StringComparison.InvariantCultureIgnoreCase)
                    select b).FirstOrDefault();
        }

        class Bundle
        {
            [JsonProperty("outputFileName")]
            public string OutputFileName { get; set; }

            [JsonProperty("inputFiles")]
            public List<string> InputFiles { get; set; } = new List<string>();
        }
    }
}
