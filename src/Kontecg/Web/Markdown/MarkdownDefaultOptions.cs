namespace Kontecg.Web.Markdown
{
    public class MarkdownDefaultOptions : IMarkdownDefaultOptions
    {
        public MarkdownDefaultOptions()
        {
            AutoHyperlink = false;
            AutoNewlines = false;
            EmptyElementSuffix = " />";
            LinkEmails = true;
            StrictBoldItalic = false;
            AsteriskIntraWordEmphasis = false;
        }

        #region Implementation of IMarkdownDefaultOptions

        /// <inheritdoc />
        public bool AutoHyperlink { get; set; }

        /// <inheritdoc />
        public bool AutoNewlines { get; set; }

        /// <inheritdoc />
        public string EmptyElementSuffix { get; set; }

        /// <inheritdoc />
        public bool LinkEmails { get; set; }

        /// <inheritdoc />
        public bool StrictBoldItalic { get; set; }

        /// <inheritdoc />
        public bool AsteriskIntraWordEmphasis { get; set; }

        #endregion
    }
}
