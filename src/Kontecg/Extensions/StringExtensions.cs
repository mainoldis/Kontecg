using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Kontecg.Collections.Extensions;

namespace Kontecg.Extensions
{
    /// <summary>
    /// Provides comprehensive extension methods for the <see cref="string"/> class to enhance
    /// string manipulation capabilities throughout the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// StringExtensions provides a rich set of utility methods that extend the functionality
    /// of the standard <see cref="string"/> class. These extensions cover common string
    /// manipulation tasks and provide consistent, reusable functionality across the application.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>String Formatting:</strong> Case conversion (camelCase, PascalCase, sentence case)</description></item>
    /// <item><description><strong>String Manipulation:</strong> Truncation, prefix/suffix management, substring operations</description></item>
    /// <item><description><strong>Validation:</strong> Null/empty checks with contract annotations</description></item>
    /// <item><description><strong>Parsing:</strong> Enum conversion, line splitting, normalization</description></item>
    /// <item><description><strong>Security:</strong> MD5 hashing for string values</description></item>
    /// <item><description><strong>Culture Support:</strong> Culture-aware string operations</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Contract Annotations:</strong> Many methods use JetBrains contract annotations
    /// to provide compile-time guarantees about null handling and return values.
    /// </para>
    /// <para>
    /// <strong>Performance:</strong> Methods are optimized for common use cases and avoid
    /// unnecessary allocations where possible.
    /// </para>
    /// </remarks>
    public static class StringExtensions
    {
        /// <summary>
        /// Ensures that a string ends with the specified character, adding it if necessary.
        /// </summary>
        /// <param name="str">
        /// The string to check and potentially modify.
        /// </param>
        /// <param name="c">
        /// The character that the string should end with.
        /// </param>
        /// <param name="comparisonType">
        /// The string comparison type to use when checking if the string ends with the character.
        /// Defaults to <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// The original string if it already ends with the specified character, or the string
        /// with the character appended if it doesn't.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the str parameter is null.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method is useful for ensuring consistent string formatting, such as ensuring
        /// file paths end with a directory separator or ensuring URLs end with a forward slash.
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> The method performs a single string comparison and
        /// only creates a new string if the character needs to be appended.
        /// </para>
        /// <para>
        /// <strong>Culture Awareness:</strong> The comparisonType parameter allows for
        /// culture-sensitive or culture-insensitive comparisons as needed.
        /// </para>
        /// </remarks>
        public static string EnsureEndsWith(this string str, char c,
            StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.EndsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return str + c;
        }

        /// <summary>
        /// Ensures that a string ends with the specified character, adding it if necessary,
        /// using culture-specific comparison.
        /// </summary>
        /// <param name="str">
        /// The string to check and potentially modify.
        /// </param>
        /// <param name="c">
        /// The character that the string should end with.
        /// </param>
        /// <param name="ignoreCase">
        /// true to ignore case during the comparison; otherwise, false.
        /// </param>
        /// <param name="culture">
        /// An object that supplies culture-specific comparison information.
        /// </param>
        /// <returns>
        /// The original string if it already ends with the specified character, or the string
        /// with the character appended if it doesn't.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the str parameter is null.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This overload provides culture-specific string comparison capabilities, allowing
        /// for proper handling of locale-specific character comparisons and casing rules.
        /// </para>
        /// <para>
        /// <strong>Culture Sensitivity:</strong> The method respects the provided culture's
        /// casing and comparison rules, making it suitable for internationalized applications.
        /// </para>
        /// </remarks>
        public static string EnsureEndsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.EndsWith(c.ToString(culture), ignoreCase, culture))
            {
                return str;
            }

            return str + c;
        }

        /// <summary>
        /// Ensures that a string starts with the specified character, adding it if necessary.
        /// </summary>
        /// <param name="str">
        /// The string to check and potentially modify.
        /// </param>
        /// <param name="c">
        /// The character that the string should start with.
        /// </param>
        /// <param name="comparisonType">
        /// The string comparison type to use when checking if the string starts with the character.
        /// Defaults to <see cref="StringComparison.Ordinal"/>.
        /// </param>
        /// <returns>
        /// The original string if it already starts with the specified character, or the string
        /// with the character prepended if it doesn't.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the str parameter is null.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method is useful for ensuring consistent string formatting, such as ensuring
        /// file paths start with a directory separator or ensuring URLs start with a forward slash.
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> The method performs a single string comparison and
        /// only creates a new string if the character needs to be prepended.
        /// </para>
        /// </remarks>
        public static string EnsureStartsWith(this string str, char c,
            StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.StartsWith(c.ToString(), comparisonType))
            {
                return str;
            }

            return c + str;
        }

        /// <summary>
        /// Ensures that a string starts with the specified character, adding it if necessary,
        /// using culture-specific comparison.
        /// </summary>
        /// <param name="str">
        /// The string to check and potentially modify.
        /// </param>
        /// <param name="c">
        /// The character that the string should start with.
        /// </param>
        /// <param name="ignoreCase">
        /// true to ignore case during the comparison; otherwise, false.
        /// </param>
        /// <param name="culture">
        /// An object that supplies culture-specific comparison information.
        /// </param>
        /// <returns>
        /// The original string if it already starts with the specified character, or the string
        /// with the character prepended if it doesn't.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the str parameter is null.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This overload provides culture-specific string comparison capabilities for string
        /// prefix validation and modification.
        /// </para>
        /// </remarks>
        public static string EnsureStartsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.StartsWith(c.ToString(culture), ignoreCase, culture))
            {
                return str;
            }

            return c + str;
        }

        /// <summary>
        /// Indicates whether the specified string is null or an empty string.
        /// </summary>
        /// <param name="str">
        /// The string to test.
        /// </param>
        /// <returns>
        /// true if the value parameter is null or an empty string (""); otherwise, false.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a convenient extension method for the standard
        /// <see cref="string.IsNullOrEmpty(string)"/> method, making it available as an
        /// instance method on string objects.
        /// </para>
        /// <para>
        /// <strong>Contract Annotation:</strong> This method is annotated with a contract
        /// that indicates it returns true when the input is null, providing compile-time
        /// guarantees for static analysis tools.
        /// </para>
        /// </remarks>
        [ContractAnnotation("null => true")]
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Indicates whether the specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="str">
        /// The string to test.
        /// </param>
        /// <returns>
        /// true if the value parameter is null or <see cref="string.Empty"/>, or if value consists
        /// exclusively of white-space characters; otherwise, false.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a convenient extension method for the standard
        /// <see cref="string.IsNullOrWhiteSpace(string)"/> method, making it available as an
        /// instance method on string objects.
        /// </para>
        /// <para>
        /// <strong>White Space:</strong> The method considers characters such as space, tab,
        /// newline, and other Unicode white-space characters as white space.
        /// </para>
        /// <para>
        /// <strong>Contract Annotation:</strong> This method is annotated with a contract
        /// that indicates it returns true when the input is null, providing compile-time
        /// guarantees for static analysis tools.
        /// </para>
        /// </remarks>
        [ContractAnnotation("null => true")]
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Gets a substring of a string from the beginning of the string.
        /// </summary>
        /// <param name="str">
        /// The source string.
        /// </param>
        /// <param name="len">
        /// The number of characters to extract from the beginning of the string.
        /// </param>
        /// <returns>
        /// A string that is equivalent to the substring of length len that begins at the
        /// start of the original string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the str parameter is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the len parameter is greater than the string's length.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to extract the leftmost characters from a string.
        /// It is equivalent to calling <see cref="string.Substring(int, int)"/> with a start
        /// index of 0.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The method validates that the length parameter does not
        /// exceed the string's actual length to prevent <see cref="ArgumentOutOfRangeException"/>.
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> The method performs a single substring operation
        /// without additional allocations.
        /// </para>
        /// </remarks>
        public static string Left(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(0, len);
        }

        /// <summary>
        /// Normalizes line endings in a string to the platform-specific line ending.
        /// </summary>
        /// <param name="str">
        /// The string to normalize.
        /// </param>
        /// <returns>
        /// A new string with all line endings normalized to <see cref="Environment.NewLine"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method converts all line ending sequences (CR, LF, CRLF) to the platform-specific
        /// line ending defined by <see cref="Environment.NewLine"/>. This is useful for ensuring
        /// consistent line endings across different operating systems.
        /// </para>
        /// <para>
        /// <strong>Line Ending Conversion:</strong>
        /// <list type="bullet">
        /// <item><description>CRLF (\r\n) → Environment.NewLine</description></item>
        /// <item><description>CR (\r) → Environment.NewLine</description></item>
        /// <item><description>LF (\n) → Environment.NewLine</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Platform Independence:</strong> The method ensures that text files and
        /// strings have consistent line endings regardless of the source platform.
        /// </para>
        /// </remarks>
        public static string NormalizeLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        }

        /// <summary>
        /// Gets the index of the nth occurrence of a character in a string.
        /// </summary>
        /// <param name="str">
        /// The source string to search.
        /// </param>
        /// <param name="c">
        /// The character to search for in the string.
        /// </param>
        /// <param name="n">
        /// The occurrence number to find (1-based).
        /// </param>
        /// <returns>
        /// The zero-based index of the nth occurrence of the character, or -1 if the character
        /// does not occur n times in the string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the str parameter is null.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method searches for the nth occurrence of a character in a string, providing
        /// a convenient way to find specific character positions beyond the first occurrence.
        /// </para>
        /// <para>
        /// <strong>Search Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>The search is case-sensitive</description></item>
        /// <item><description>The occurrence count is 1-based (first occurrence is n=1)</description></item>
        /// <item><description>Returns -1 if the character occurs fewer than n times</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> The method performs a linear search through the string,
        /// stopping when the nth occurrence is found.
        /// </para>
        /// </remarks>
        public static int NthIndexOf(this string str, char c, int n)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] != c)
                {
                    continue;
                }

                if (++count == n)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes the first occurrence of any of the given postfixes from the end of the string.
        /// </summary>
        /// <param name="str">
        /// The string to process.
        /// </param>
        /// <param name="postFixes">
        /// One or more postfixes to remove. Order is important - if one postfix matches,
        /// others will not be tested.
        /// </param>
        /// <returns>
        /// The modified string with the first matching postfix removed, or the original string
        /// if none of the postfixes match.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method removes the first matching postfix from the end of a string. It tests
        /// postfixes in the order they are provided, and stops at the first match.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>Returns null if the input string is null</description></item>
        /// <item><description>Returns empty string if the input string is empty</description></item>
        /// <item><description>Returns the original string if no postfixes are provided</description></item>
        /// <item><description>Tests postfixes in order and stops at the first match</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for removing file extensions, URL suffixes,
        /// or other trailing patterns from strings.
        /// </para>
        /// </remarks>
        public static string RemovePostFix(this string str, params string[] postFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (postFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (string postFix in postFixes)
            {
                if (str.EndsWith(postFix))
                {
                    return str.Left(str.Length - postFix.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// Removes the first occurrence of any of the given prefixes from the beginning of the string.
        /// </summary>
        /// <param name="str">
        /// The string to process.
        /// </param>
        /// <param name="preFixes">
        /// One or more prefixes to remove. Order is important - if one prefix matches,
        /// others will not be tested.
        /// </param>
        /// <returns>
        /// The modified string with the first matching prefix removed, or the original string
        /// if none of the prefixes match.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method removes the first matching prefix from the beginning of a string. It tests
        /// prefixes in the order they are provided, and stops at the first match.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>Returns null if the input string is null</description></item>
        /// <item><description>Returns empty string if the input string is empty</description></item>
        /// <item><description>Returns the original string if no prefixes are provided</description></item>
        /// <item><description>Tests prefixes in order and stops at the first match</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for removing protocol prefixes from URLs,
        /// removing leading slashes from paths, or other leading pattern removal.
        /// </para>
        /// </remarks>
        public static string RemovePreFix(this string str, params string[] preFixes)
        {
            if (str == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            if (preFixes.IsNullOrEmpty())
            {
                return str;
            }

            foreach (string preFix in preFixes)
            {
                if (str.StartsWith(preFix))
                {
                    return str.Right(str.Length - preFix.Length);
                }
            }

            return str;
        }

        /// <summary>
        /// Gets a substring of a string from the end of the string.
        /// </summary>
        /// <param name="str">
        /// The source string.
        /// </param>
        /// <param name="len">
        /// The number of characters to extract from the end of the string.
        /// </param>
        /// <returns>
        /// A string that is equivalent to the substring of length len that ends at the
        /// end of the original string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the str parameter is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the len parameter is greater than the string's length.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to extract the rightmost characters from a string.
        /// It is equivalent to calling <see cref="string.Substring(int, int)"/> with a start
        /// index calculated from the end of the string.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The method validates that the length parameter does not
        /// exceed the string's actual length to prevent <see cref="ArgumentOutOfRangeException"/>.
        /// </para>
        /// </remarks>
        public static string Right(this string str, int len)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            if (str.Length < len)
            {
                throw new ArgumentException("len argument can not be bigger than given string's length!");
            }

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// Splits a string by a specified separator using the default split options.
        /// </summary>
        /// <param name="str">
        /// The string to split.
        /// </param>
        /// <param name="separator">
        /// The string that delimits the substrings in the original string.
        /// </param>
        /// <returns>
        /// An array whose elements contain the substrings from the original string that are
        /// delimited by the separator.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to split strings by a specific separator
        /// without having to create a separator array. It uses <see cref="StringSplitOptions.None"/>
        /// as the default split option.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong> Empty substrings are included in the result array,
        /// and the method preserves all whitespace in the substrings.
        /// </para>
        /// </remarks>
        public static string[] Split(this string str, string separator)
        {
            return str.Split(new[] {separator}, StringSplitOptions.None);
        }

        /// <summary>
        /// Splits a string by a specified separator using the specified split options.
        /// </summary>
        /// <param name="str">
        /// The string to split.
        /// </param>
        /// <param name="separator">
        /// The string that delimits the substrings in the original string.
        /// </param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that specifies whether to trim
        /// substrings and include empty substrings.
        /// </param>
        /// <returns>
        /// An array whose elements contain the substrings from the original string that are
        /// delimited by the separator.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to split strings by a specific separator
        /// with control over the split behavior through the options parameter.
        /// </para>
        /// <para>
        /// <strong>Options:</strong> The options parameter allows control over whether
        /// empty substrings are included and whether substrings are trimmed of whitespace.
        /// </para>
        /// </remarks>
        public static string[] Split(this string str, string separator, StringSplitOptions options)
        {
            return str.Split(new[] {separator}, options);
        }

        /// <summary>
        /// Splits a string into lines using the platform-specific line ending.
        /// </summary>
        /// <param name="str">
        /// The string to split into lines.
        /// </param>
        /// <returns>
        /// An array whose elements contain the lines from the original string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method splits a string into lines using <see cref="Environment.NewLine"/>
        /// as the delimiter. It provides a convenient way to process text line by line.
        /// </para>
        /// <para>
        /// <strong>Line Handling:</strong> Empty lines are included in the result array,
        /// and the method preserves all whitespace within each line.
        /// </para>
        /// </remarks>
        public static string[] SplitToLines(this string str)
        {
            return str.Split(Environment.NewLine);
        }

        /// <summary>
        /// Splits a string into lines using the platform-specific line ending with specified options.
        /// </summary>
        /// <param name="str">
        /// The string to split into lines.
        /// </param>
        /// <param name="options">
        /// A bitwise combination of the enumeration values that specifies whether to trim
        /// lines and include empty lines.
        /// </param>
        /// <returns>
        /// An array whose elements contain the lines from the original string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides control over line splitting behavior through the options parameter,
        /// allowing for more flexible text processing.
        /// </para>
        /// </remarks>
        public static string[] SplitToLines(this string str, StringSplitOptions options)
        {
            return str.Split(Environment.NewLine, options);
        }

        /// <summary>
        /// Converts a PascalCase string to camelCase.
        /// </summary>
        /// <param name="str">
        /// The string to convert.
        /// </param>
        /// <param name="invariantCulture">
        /// true to use invariant culture for case conversion; otherwise, false.
        /// Defaults to true.
        /// </param>
        /// <returns>
        /// The camelCase version of the input string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method converts the first character of a string to lowercase while preserving
        /// the rest of the string. It is commonly used for converting property names from
        /// PascalCase to camelCase for JSON serialization or JavaScript compatibility.
        /// </para>
        /// <para>
        /// <strong>Examples:</strong>
        /// <list type="bullet">
        /// <item><description>"FirstName" → "firstName"</description></item>
        /// <item><description>"UserID" → "userID"</description></item>
        /// <item><description>"API" → "aPI"</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Culture Handling:</strong> The invariantCulture parameter determines
        /// whether to use culture-invariant or culture-specific case conversion rules.
        /// </para>
        /// </remarks>
        public static string ToCamelCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return invariantCulture ? str.ToLowerInvariant() : str.ToLower();
            }

            return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str.Substring(1);
        }

        /// <summary>
        /// Converts a PascalCase string to camelCase using the specified culture.
        /// </summary>
        /// <param name="str">
        /// The string to convert.
        /// </param>
        /// <param name="culture">
        /// An object that supplies culture-specific casing rules.
        /// </param>
        /// <returns>
        /// The camelCase version of the input string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This overload provides culture-specific case conversion capabilities, allowing
        /// for proper handling of locale-specific casing rules.
        /// </para>
        /// </remarks>
        public static string ToCamelCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToLower(culture);
            }

            return char.ToLower(str[0], culture) + str.Substring(1);
        }

        /// <summary>
        /// Converts a PascalCase or camelCase string to sentence case by adding spaces
        /// between words and converting to proper case.
        /// </summary>
        /// <param name="str">
        /// The string to convert.
        /// </param>
        /// <param name="invariantCulture">
        /// true to use invariant culture for case conversion; otherwise, false.
        /// Defaults to false.
        /// </param>
        /// <returns>
        /// The sentence case version of the input string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method uses regular expressions to identify word boundaries in PascalCase
        /// or camelCase strings and converts them to sentence case by adding spaces and
        /// converting to proper case.
        /// </para>
        /// <para>
        /// <strong>Examples:</strong>
        /// <list type="bullet">
        /// <item><description>"ThisIsSampleSentence" → "This is a sample sentence"</description></item>
        /// <item><description>"userName" → "user name"</description></item>
        /// <item><description>"APIEndpoint" → "API endpoint"</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Pattern Matching:</strong> The method uses the regex pattern "[a-z][A-Z]"
        /// to find lowercase letters followed by uppercase letters, indicating word boundaries.
        /// </para>
        /// </remarks>
        public static string ToSentenceCase(this string str, bool invariantCulture = false)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return Regex.Replace(
                str,
                "[a-z][A-Z]",
                m => m.Value[0] + " " +
                     (invariantCulture ? char.ToLowerInvariant(m.Value[1]) : char.ToLower(m.Value[1]))
            );
        }

        /// <summary>
        /// Converts a PascalCase or camelCase string to sentence case using the specified culture.
        /// </summary>
        /// <param name="str">
        /// The string to convert.
        /// </param>
        /// <param name="culture">
        /// An object that supplies culture-specific casing rules.
        /// </param>
        /// <returns>
        /// The sentence case version of the input string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This overload provides culture-specific case conversion capabilities for sentence
        /// case transformation.
        /// </para>
        /// </remarks>
        public static string ToSentenceCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1], culture));
        }

        /// <summary>
        /// Converts a string to an enum value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum to convert to.
        /// </typeparam>
        /// <param name="value">
        /// The string value to convert.
        /// </param>
        /// <returns>
        /// The enum value corresponding to the string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the value parameter is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the string value does not correspond to any enum value.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to convert string values to enum types
        /// using case-sensitive comparison. It is equivalent to calling
        /// <see cref="Enum.Parse(Type, string)"/> with the enum type.
        /// </para>
        /// <para>
        /// <strong>Case Sensitivity:</strong> The conversion is case-sensitive by default.
        /// Use the overload with ignoreCase parameter for case-insensitive conversion.
        /// </para>
        /// <para>
        /// <strong>Error Handling:</strong> The method throws an <see cref="ArgumentException"/>
        /// if the string value does not correspond to any defined enum value.
        /// </para>
        /// </remarks>
        public static T ToEnum<T>(this string value)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return (T) Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// Converts a string to an enum value with optional case-insensitive comparison.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the enum to convert to.
        /// </typeparam>
        /// <param name="value">
        /// The string value to convert.
        /// </param>
        /// <param name="ignoreCase">
        /// true to ignore case during the conversion; otherwise, false.
        /// </param>
        /// <returns>
        /// The enum value corresponding to the string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the value parameter is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the string value does not correspond to any enum value.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This overload provides control over case sensitivity during enum conversion,
        /// making it more flexible for user input or configuration scenarios.
        /// </para>
        /// </remarks>
        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return (T) Enum.Parse(typeof(T), value, ignoreCase);
        }

        /// <summary>
        /// Computes the MD5 hash of a string and returns it as a hexadecimal string.
        /// </summary>
        /// <param name="str">
        /// The string to hash.
        /// </param>
        /// <returns>
        /// A 32-character hexadecimal string representing the MD5 hash of the input string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method computes the MD5 hash of a string using UTF-8 encoding and returns
        /// the result as a hexadecimal string. The hash is always 32 characters long.
        /// </para>
        /// <para>
        /// <strong>Encoding:</strong> The string is converted to bytes using UTF-8 encoding
        /// before hashing, ensuring consistent results across different platforms.
        /// </para>
        /// <para>
        /// <strong>Security Note:</strong> MD5 is not considered cryptographically secure
        /// for security applications. Use SHA-256 or other secure hash algorithms for
        /// password hashing or digital signatures.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> MD5 is still useful for checksums, file integrity
        /// verification, and other non-security-critical applications.
        /// </para>
        /// </remarks>
        public static string ToMd5(this string str)
        {
            using MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a camelCase string to PascalCase.
        /// </summary>
        /// <param name="str">
        /// The string to convert.
        /// </param>
        /// <param name="invariantCulture">
        /// true to use invariant culture for case conversion; otherwise, false.
        /// Defaults to true.
        /// </param>
        /// <returns>
        /// The PascalCase version of the input string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method converts the first character of a string to uppercase while preserving
        /// the rest of the string. It is commonly used for converting property names from
        /// camelCase to PascalCase for C# naming conventions.
        /// </para>
        /// <para>
        /// <strong>Examples:</strong>
        /// <list type="bullet">
        /// <item><description>"firstName" → "FirstName"</description></item>
        /// <item><description>"userID" → "UserID"</description></item>
        /// <item><description>"api" → "Api"</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Culture Handling:</strong> The invariantCulture parameter determines
        /// whether to use culture-invariant or culture-specific case conversion rules.
        /// </para>
        /// </remarks>
        public static string ToPascalCase(this string str, bool invariantCulture = true)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return invariantCulture ? str.ToUpperInvariant() : str.ToUpper();
            }

            return (invariantCulture ? char.ToUpperInvariant(str[0]) : char.ToUpper(str[0])) + str.Substring(1);
        }

        /// <summary>
        /// Converts a camelCase string to PascalCase using the specified culture.
        /// </summary>
        /// <param name="str">
        /// The string to convert.
        /// </param>
        /// <param name="culture">
        /// An object that supplies culture-specific casing rules.
        /// </param>
        /// <returns>
        /// The PascalCase version of the input string.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This overload provides culture-specific case conversion capabilities for
        /// PascalCase transformation.
        /// </para>
        /// </remarks>
        public static string ToPascalCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper(culture);
            }

            return char.ToUpper(str[0], culture) + str.Substring(1);
        }

        /// <summary>
        /// Truncates a string to the specified maximum length.
        /// </summary>
        /// <param name="str">
        /// The string to truncate.
        /// </param>
        /// <param name="maxLength">
        /// The maximum length of the resulting string.
        /// </param>
        /// <returns>
        /// The truncated string, or the original string if it is shorter than maxLength.
        /// Returns null if the input string is null.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method truncates a string to the specified maximum length by taking the
        /// leftmost characters. If the string is already shorter than maxLength, it is
        /// returned unchanged.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>Returns null if the input string is null</description></item>
        /// <item><description>Returns the original string if it is shorter than maxLength</description></item>
        /// <item><description>Truncates from the beginning of the string</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for limiting display text length,
        /// database field constraints, or API response truncation.
        /// </para>
        /// </remarks>
        public static string Truncate(this string str, int maxLength)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            return str.Left(maxLength);
        }

        /// <summary>
        /// Truncates a string to the specified maximum length and adds a postfix if truncated.
        /// </summary>
        /// <param name="str">
        /// The string to truncate.
        /// </param>
        /// <param name="maxLength">
        /// The maximum length of the resulting string.
        /// </param>
        /// <param name="postfix">
        /// The string to append if the original string is truncated. Defaults to "...".
        /// </param>
        /// <returns>
        /// The truncated string with postfix if necessary, or the original string if it
        /// fits within maxLength. Returns null if the input string is null.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method truncates a string and adds a postfix (typically ellipsis) to indicate
        /// that the string has been shortened. The total length of the result will not exceed maxLength.
        /// </para>
        /// <para>
        /// <strong>Behavior:</strong>
        /// <list type="bullet">
        /// <item><description>Returns null if the input string is null</description></item>
        /// <item><description>Returns empty string if the input string is empty or maxLength is 0</description></item>
        /// <item><description>Returns the original string if it fits within maxLength</description></item>
        /// <item><description>Truncates and adds postfix if the string is too long</description></item>
        /// <item><description>If maxLength is too short for the postfix, truncates the postfix itself</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for UI text truncation where you want
        /// to indicate that content has been cut off, such as in tooltips, summaries, or
        /// preview text.
        /// </para>
        /// </remarks>
        public static string TruncateWithPostfix(this string str, int maxLength, string postfix = "...")
        {
            if (str == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(str) || maxLength == 0)
            {
                return string.Empty;
            }

            if (str.Length <= maxLength)
            {
                return str;
            }

            if (maxLength <= postfix.Length)
            {
                return postfix.Left(maxLength);
            }

            return str.Left(maxLength - postfix.Length) + postfix;
        }
    }
}
