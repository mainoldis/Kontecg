namespace Kontecg.Text.Formatting
{
    internal class FormatStringToken
    {
        public FormatStringToken(string text, FormatStringTokenType type)
        {
            Text = text;
            Type = type;
        }

        public string Text { get; }

        public FormatStringTokenType Type { get; }
    }
}
