namespace Kontecg.Web.Markdown
{
    public interface IMarkdownPreprocessor
    {
        string Transform(string text, MarkdownOptions options);
    }
}
