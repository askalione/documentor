using Markdig;

namespace Documentor.Services.MarkdownConverters
{
    public class MarkdigMarkdownConverter : IMarkdownConverter
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdigMarkdownConverter()
        {
            _pipeline = new MarkdownPipelineBuilder()
                 .UseBootstrap()
                 .UsePipeTables()
                 .UseAutoIdentifiers(Markdig.Extensions.AutoIdentifiers.AutoIdentifierOptions.GitHub)
                 .UseReferralLinks("nofollow")
                 .Build();
        }

        public string ConvertToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown, _pipeline);
        }
    }
}
