using JetBrains.Annotations;
using Kontecg.Extensions;

namespace Kontecg.Web.Markdown
{
    /// <summary>
    ///     Markdown options.
    /// </summary>
    public class MarkdownOptions
    {
        /// <summary>
        ///     when true, (most) bare plain URLs are auto-hyperlink-ed
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool? AutoHyperlink { get; private set; }

        /// <summary>
        ///     when true, RETURN becomes a literal newline
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool? AutoNewlines { get; private set; }

        /// <summary>
        ///     use ">" for HTML output, or " />" for XHTML output
        /// </summary>
        [CanBeNull]
        public string EmptyElementSuffix { get; private set; }

        /// <summary>
        ///     when false, email addresses will never be auto-linked
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool? LinkEmails { get; private set; }

        /// <summary>
        ///     when true, bold and italic require non-word characters on either side
        ///     WARNING: this is a significant deviation from the markdown spec
        /// </summary>
        public bool? StrictBoldItalic { get; private set; }

        /// <summary>
        ///     when true, asterisks may be used for intra-word emphasis
        ///     this does nothing if StrictBoldItalic is false
        /// </summary>
        public bool? AsteriskIntraWordEmphasis { get; private set; }

        internal void FillDefaultsForNonProvidedOptions(IMarkdownDefaultOptions defaultOptions)
        {
            AutoHyperlink ??= defaultOptions.AutoHyperlink;
            AutoNewlines ??= defaultOptions.AutoNewlines;
            EmptyElementSuffix = EmptyElementSuffix.IsNullOrWhiteSpace()
                ? defaultOptions.EmptyElementSuffix
                : EmptyElementSuffix;
            LinkEmails ??= defaultOptions.LinkEmails;
            StrictBoldItalic ??= defaultOptions.StrictBoldItalic;
            AsteriskIntraWordEmphasis ??= defaultOptions.AsteriskIntraWordEmphasis;
        }
    }
}
