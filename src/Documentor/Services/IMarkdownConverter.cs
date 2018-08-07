namespace Documentor.Services
{
    public interface IMarkdownConverter
    {
        string ConvertToHtml(string markdown);
    }
}
