using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Kontecg.Collections.Extensions;
using Kontecg.Extensions;

namespace Kontecg
{
    /// <summary>
    /// Provides static utility methods for parameter validation and null checking operations.
    /// This class implements defensive programming patterns to ensure method preconditions are met.
    /// </summary>
    /// <remarks>
    /// The Check class is designed to be used at the beginning of methods to validate parameters
    /// and throw appropriate exceptions when validation fails. It uses JetBrains annotations
    /// for static analysis and provides compile-time guarantees about null safety.
    /// </remarks>
    [DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// Validates that a parameter is not null and throws an ArgumentNullException if it is.
        /// </summary>
        /// <typeparam name="T">The type of the parameter to validate.</typeparam>
        /// <param name="value">The parameter value to check for null.</param>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <returns>The original value if it is not null.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the value parameter is null.</exception>
        /// <remarks>
        /// This method is marked with ContractAnnotation to provide static analysis hints
        /// that the method will halt execution if the value is null.
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static T NotNull<T>(T value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }

        /// <summary>
        /// Validates that a string parameter is not null or empty and throws an ArgumentException if it is.
        /// </summary>
        /// <param name="value">The string parameter to validate.</param>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <returns>The original string value if it is not null or empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the value parameter is null or empty.</exception>
        /// <remarks>
        /// This method uses the IsNullOrEmpty extension method to perform the validation.
        /// It is commonly used for validating required string parameters in public APIs.
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrEmpty(string value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException($"{parameterName} can not be null or empty!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// Validates that a string parameter is not null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string parameter to validate.</param>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <returns>The original string value if it contains meaningful content.</returns>
        /// <exception cref="ArgumentException">Thrown when the value parameter is null, empty, or white-space only.</exception>
        /// <remarks>
        /// This method is more restrictive than NotNullOrEmpty as it also checks for white-space only strings.
        /// It is useful for validating user input that should contain actual content.
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static string NotNullOrWhiteSpace(string value, [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value.IsNullOrWhiteSpace())
            {
                throw new ArgumentException($"{parameterName} can not be null, empty or white space!", parameterName);
            }

            return value;
        }

        /// <summary>
        /// Validates that a collection parameter is not null or empty and throws an ArgumentException if it is.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="value">The collection parameter to validate.</param>
        /// <param name="parameterName">The name of the parameter being validated.</param>
        /// <returns>The original collection value if it is not null or empty.</returns>
        /// <exception cref="ArgumentException">Thrown when the value parameter is null or empty.</exception>
        /// <remarks>
        /// This method validates both that the collection reference is not null and that it contains at least one element.
        /// It is commonly used for validating required collection parameters in public APIs.
        /// </remarks>
        [ContractAnnotation("value:null => halt")]
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> value,
            [InvokerParameterName] [NotNull] string parameterName)
        {
            if (value.IsNullOrEmpty())
            {
                throw new ArgumentException(parameterName + " can not be null or empty!", parameterName);
            }

            return value;
        }
    }
}
