namespace Documentor.Services.MarkdownConverters
{
    public interface IMarkdownConverter
    {
        string ConvertToHtml(string markdown);
    }
}
