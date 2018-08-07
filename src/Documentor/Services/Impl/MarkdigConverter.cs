using Markdig;

namespace Documentor.Services.Impl
{
    public class MarkdigConverter : IMarkdownConverter
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdigConverter()
        {
            _pipeline = new MarkdownPipelineBuilder()
                 .UseBootstrap()
                 .UsePipeTables()
                 .Build();
        }

        public string ConvertToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown, _pipeline);
        }
    }
}
