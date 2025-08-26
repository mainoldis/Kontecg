using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Kontecg.Extensions
{
    /// <summary>
    /// Provides comprehensive extension methods for all objects to enhance type conversion,
    /// casting, and utility operations throughout the Kontecg framework.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ObjectExtensions provides a rich set of utility methods that extend the functionality
    /// of all objects in the .NET framework. These extensions cover common object manipulation
    /// tasks and provide consistent, reusable functionality across the application.
    /// </para>
    /// <para>
    /// <strong>Key Features:</strong>
    /// <list type="bullet">
    /// <item><description><strong>Type Casting:</strong> Simplified and beautified casting operations</description></item>
    /// <item><description><strong>Type Conversion:</strong> Flexible conversion between different types</description></item>
    /// <item><description><strong>Enum Support:</strong> Specialized handling for enum conversions</description></item>
    /// <item><description><strong>Collection Operations:</strong> Utility methods for list membership checking</description></item>
    /// <item><description><strong>Special Types:</strong> Support for Guid, TimeSpan, and other special types</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Type Safety:</strong> Methods provide compile-time type checking while offering
    /// convenient syntax for common object operations.
    /// </para>
    /// <para>
    /// <strong>Culture Invariance:</strong> Conversions use invariant culture to ensure
    /// consistent behavior across different locales and environments.
    /// </para>
    /// </remarks>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Simplifies and beautifies casting an object to a specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The target type to cast the object to. Must be a reference type.
        /// </typeparam>
        /// <param name="obj">
        /// The object to cast.
        /// </param>
        /// <returns>
        /// The object cast to the specified type.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a more elegant syntax for casting objects to specific types.
        /// It is equivalent to using the C# cast operator but with a more fluent API.
        /// </para>
        /// <para>
        /// <strong>Type Constraint:</strong> The generic type parameter T must be a reference
        /// type (class), as this method performs reference type casting.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used when you need to cast objects in a
        /// more readable way, especially in LINQ queries or when working with generic
        /// collections.
        /// </para>
        /// <para>
        /// <strong>Example:</strong>
        /// <code>
        /// object someObject = "Hello World";
        /// string result = someObject.As&lt;string&gt;();
        /// </code>
        /// </para>
        /// <para>
        /// <strong>Note:</strong> This method performs a direct cast and will throw an
        /// <see cref="InvalidCastException"/> if the cast is not valid.
        /// </para>
        /// </remarks>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T) obj;
        }

        /// <summary>
        /// Converts an object to a value type or enum using type conversion mechanisms.
        /// </summary>
        /// <typeparam name="T">
        /// The target value type to convert the object to.
        /// </typeparam>
        /// <param name="obj">
        /// The object to convert.
        /// </param>
        /// <returns>
        /// The object converted to the specified value type.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when converting to an enum type that is not defined for the given value.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Thrown when the conversion cannot be performed.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method provides flexible type conversion for value types and enums. It uses
        /// different conversion strategies depending on the target type:
        /// </para>
        /// <para>
        /// <strong>Conversion Strategies:</strong>
        /// <list type="bullet">
        /// <item><description><strong>Guid and TimeSpan:</strong> Uses <see cref="TypeDescriptor.GetConverter"/> with invariant culture</description></item>
        /// <item><description><strong>Enums:</strong> Uses <see cref="Enum.Parse"/> with validation</description></item>
        /// <item><description><strong>Other Types:</strong> Uses <see cref="Convert.ChangeType"/> with invariant culture</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Enum Handling:</strong> For enum conversions, the method validates that
        /// the value is defined in the enum before performing the conversion. If the value
        /// is not defined, an <see cref="ArgumentException"/> is thrown.
        /// </para>
        /// <para>
        /// <strong>Culture Invariance:</strong> All conversions use invariant culture to
        /// ensure consistent behavior across different locales.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for configuration parsing, data binding,
        /// and when working with weakly typed data that needs to be converted to strongly
        /// typed values.
        /// </para>
        /// <para>
        /// <strong>Examples:</strong>
        /// <code>
        /// object intValue = "42";
        /// int result = intValue.To&lt;int&gt;(); // Returns 42
        /// 
        /// object enumValue = "Active";
        /// Status status = enumValue.To&lt;Status&gt;(); // Converts to Status.Active
        /// 
        /// object guidValue = "12345678-1234-1234-1234-123456789012";
        /// Guid guid = guidValue.To&lt;Guid&gt;(); // Converts to Guid
        /// </code>
        /// </para>
        /// </remarks>
        public static T To<T>(this object obj)
            where T : struct
        {
            if (typeof(T) == typeof(Guid) || typeof(T) == typeof(TimeSpan))
            {
                return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            if (typeof(T).IsEnum)
            {
                if (Enum.IsDefined(typeof(T), obj))
                {
                    return (T) Enum.Parse(typeof(T), obj.ToString());
                }

                throw new ArgumentException($"Enum type undefined '{obj}'.");
            }

            return (T) Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Checks if an item is present in a specified list of items.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the items to check.
        /// </typeparam>
        /// <param name="item">
        /// The item to check for in the list.
        /// </param>
        /// <param name="list">
        /// The list of items to search in.
        /// </param>
        /// <returns>
        /// true if the item is found in the list; otherwise, false.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a convenient way to check if an item exists in a collection
        /// of items. It uses <see cref="Enumerable.Contains{T}(System.Collections.Generic.IEnumerable{T}, T)"/>
        /// to perform the check.
        /// </para>
        /// <para>
        /// <strong>Comparison:</strong> The method uses the default equality comparer for
        /// the type T to determine if the item is in the list.
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> For small lists, this method is efficient. For
        /// large lists, consider using a <see cref="System.Collections.Generic.HashSet{T}"/>
        /// for better performance.
        /// </para>
        /// <para>
        /// <strong>Use Cases:</strong> Commonly used for validation, filtering, and when
        /// you need to check if a value is one of several allowed values.
        /// </para>
        /// <para>
        /// <strong>Examples:</strong>
        /// <code>
        /// string status = "Active";
        /// bool isValid = status.IsIn("Active", "Inactive", "Pending"); // Returns true
        /// 
        /// int value = 5;
        /// bool isSpecial = value.IsIn(1, 3, 5, 7, 9); // Returns true
        /// 
        /// DayOfWeek day = DayOfWeek.Monday;
        /// bool isWeekend = day.IsIn(DayOfWeek.Saturday, DayOfWeek.Sunday); // Returns false
        /// </code>
        /// </para>
        /// </remarks>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }
    }
}
