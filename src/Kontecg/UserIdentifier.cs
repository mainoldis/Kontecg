using System;
using System.Reflection;
using Kontecg.Extensions;

namespace Kontecg
{
    /// <summary>
    /// Represents a unique identifier for a user within the Kontecg multi-company system.
    /// This class encapsulates both user and company identification for proper data isolation
    /// and user context management.
    /// </summary>
    /// <remarks>
    /// <para>
    /// UserIdentifier is a value object that uniquely identifies a user within the system.
    /// It supports both single-company and multi-company scenarios by optionally including
    /// a company identifier. This design allows for proper data isolation and user context
    /// management in multi-tenant applications.
    /// </para>
    /// <para>
    /// <strong>Multi-Company Support:</strong> The class supports two types of users:
    /// <list type="bullet">
    /// <item><description><strong>Company Users:</strong> Users associated with a specific company (CompanyId is not null)</description></item>
    /// <item><description><strong>Host Users:</strong> System-level users not tied to any specific company (CompanyId is null)</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>String Format:</strong> The class supports parsing and serialization to/from strings:
    /// <list type="bullet">
    /// <item><description><strong>Company Users:</strong> "userId@companyId" (e.g., "42@3")</description></item>
    /// <item><description><strong>Host Users:</strong> "userId" (e.g., "1")</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <strong>Value Object Semantics:</strong> This class implements value object patterns with
    /// proper equality comparison, hash code generation, and immutability to ensure consistent
    /// behavior in collections and comparisons.
    /// </para>
    /// <para>
    /// <strong>Serialization:</strong> The class is marked as serializable to support
    /// persistence, caching, and cross-process communication scenarios.
    /// </para>
    /// </remarks>
    [Serializable]
    public class UserIdentifier : IUserIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentifier"/> class with the specified
        /// company and user identifiers.
        /// </summary>
        /// <param name="companyId">
        /// The company identifier. Can be null for host users in a multi-company application.
        /// </param>
        /// <param name="userId">
        /// The unique identifier of the user within the system.
        /// </param>
        /// <remarks>
        /// <para>
        /// This constructor creates a UserIdentifier for either a company user or a host user.
        /// For company users, both companyId and userId must be provided. For host users,
        /// companyId should be null.
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The constructor does not perform validation on the parameters.
        /// It is the responsibility of the caller to ensure valid values are provided.
        /// </para>
        /// </remarks>
        public UserIdentifier(int? companyId, long userId)
        {
            CompanyId = companyId;
            UserId = userId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserIdentifier"/> class.
        /// This protected constructor is used for serialization and inheritance scenarios.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor is provided to support serialization frameworks and inheritance.
        /// It initializes the UserIdentifier with default values that should be set
        /// through other means (e.g., property setters or derived class constructors).
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> This constructor should generally not be used directly
        /// in application code. Use the public constructor instead.
        /// </para>
        /// </remarks>
        protected UserIdentifier()
        {
        }

        /// <summary>
        /// Gets the company identifier associated with this user.
        /// </summary>
        /// <value>
        /// The company identifier, or null if this is a host user.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property identifies the company that the user belongs to in a multi-company
        /// application. For company users, this value will be a valid company identifier.
        /// For host users (system-level users), this value will be null.
        /// </para>
        /// <para>
        /// <strong>Multi-Company Context:</strong> This property is used for data isolation
        /// and access control in multi-tenant applications. It ensures that users can only
        /// access data belonging to their associated company.
        /// </para>
        /// <para>
        /// <strong>Access Level:</strong> The setter is protected to maintain immutability
        /// while allowing inheritance and serialization scenarios.
        /// </para>
        /// </remarks>
        public int? CompanyId { get; protected set; }

        /// <summary>
        /// Gets the unique identifier of the user within the system.
        /// </summary>
        /// <value>
        /// The unique user identifier.
        /// </value>
        /// <remarks>
        /// <para>
        /// This property contains the unique identifier that distinguishes this user from
        /// all other users in the system. The UserId is unique across the entire application,
        /// regardless of company association.
        /// </para>
        /// <para>
        /// <strong>Uniqueness:</strong> The UserId must be unique across the entire system
        /// to ensure proper user identification and prevent conflicts.
        /// </para>
        /// <para>
        /// <strong>Access Level:</strong> The setter is protected to maintain immutability
        /// while allowing inheritance and serialization scenarios.
        /// </para>
        /// </remarks>
        public long UserId { get; protected set; }

        /// <summary>
        /// Parses a string representation and creates a new <see cref="UserIdentifier"/> object.
        /// </summary>
        /// <param name="userIdentifierString">
        /// The string representation of the user identifier. Should be formatted as one of the following:
        /// <list type="bullet">
        /// <item><description>"userId@companyId" - for company users (e.g., "42@3")</description></item>
        /// <item><description>"userId" - for host users (e.g., "1")</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// A new <see cref="UserIdentifier"/> instance representing the parsed string.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the userIdentifierString parameter is null or empty.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when the userIdentifierString is not properly formatted.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This static method parses a string representation of a UserIdentifier and creates
        /// a corresponding object instance. The method supports both company users and host users.
        /// </para>
        /// <para>
        /// <strong>Format Rules:</strong>
        /// <list type="bullet">
        /// <item><description>Company users must use the format "userId@companyId"</description></item>
        /// <item><description>Host users must use the format "userId" (without company identifier)</description></item>
        /// <item><description>Both userId and companyId must be valid numeric values</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Validation:</strong> The method validates the input string format and throws
        /// appropriate exceptions for invalid input.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> This method is commonly used when deserializing user identifiers
        /// from strings, such as from configuration files, URLs, or external systems.
        /// </para>
        /// </remarks>
        public static UserIdentifier Parse(string userIdentifierString)
        {
            if (userIdentifierString.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(userIdentifierString),
                    "userAtCompany can not be null or empty!");
            }

            string[] splitted = userIdentifierString.Split('@');
            if (splitted.Length == 1)
            {
                return new UserIdentifier(null, splitted[0].To<long>());
            }

            if (splitted.Length == 2)
            {
                return new UserIdentifier(splitted[1].To<int>(), splitted[0].To<long>());
            }

            throw new ArgumentException("userAtCompany is not properly formatted", nameof(userIdentifierString));
        }

        /// <summary>
        /// Creates a string representation of this <see cref="UserIdentifier"/> instance.
        /// </summary>
        /// <returns>
        /// A string representation that can be used to recreate this UserIdentifier object.
        /// The format depends on the user type:
        /// <list type="bullet">
        /// <item><description>"userId@companyId" - for company users (e.g., "42@3")</description></item>
        /// <item><description>"userId" - for host users (e.g., "1")</description></item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method creates a string representation of the UserIdentifier that can be used
        /// with the <see cref="Parse"/> method to recreate an identical UserIdentifier object.
        /// </para>
        /// <para>
        /// <strong>Format:</strong> The returned string format depends on whether this is a
        /// company user or a host user. Company users include both the user ID and company ID,
        /// while host users only include the user ID.
        /// </para>
        /// <para>
        /// <strong>Reversibility:</strong> The string representation is designed to be reversible,
        /// meaning that parsing the result of this method will produce an equivalent UserIdentifier
        /// object.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> This method is commonly used for serialization, logging,
        /// and when passing user identifiers as strings to external systems or APIs.
        /// </para>
        /// </remarks>
        public string ToUserIdentifierString()
        {
            if (CompanyId == null)
            {
                return UserId.ToString();
            }

            return UserId + "@" + CompanyId;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current UserIdentifier.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with the current UserIdentifier.
        /// </param>
        /// <returns>
        /// true if the specified object is equal to the current UserIdentifier; otherwise, false.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method implements value equality for UserIdentifier objects. Two UserIdentifier
        /// instances are considered equal if they have the same UserId and CompanyId values.
        /// </para>
        /// <para>
        /// <strong>Equality Rules:</strong>
        /// <list type="bullet">
        /// <item><description>Same instance references are always equal</description></item>
        /// <item><description>Different types are never equal (unless they have an inheritance relationship)</description></item>
        /// <item><description>Both UserId and CompanyId must match for equality</description></item>
        /// <item><description>Null CompanyId values are considered equal to other null CompanyId values</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Type Compatibility:</strong> The method supports comparison between UserIdentifier
        /// and derived types, as long as they have an inheritance relationship.
        /// </para>
        /// </remarks>
        public override bool Equals(object obj)
        {
            if (!(obj is UserIdentifier))
            {
                return false;
            }

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            //Transient objects are not considered as equal
            UserIdentifier other = (UserIdentifier) obj;

            //Must have a IS-A relation of types or must be same type
            Type typeOfThis = GetType();
            Type typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) &&
                !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return CompanyId == other.CompanyId && UserId == other.UserId;
        }

        /// <summary>
        /// Serves as a hash function for the UserIdentifier type.
        /// </summary>
        /// <returns>
        /// A hash code for the current UserIdentifier object.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method generates a hash code that is consistent with the equality implementation.
        /// The hash code is based on both the UserId and CompanyId values.
        /// </para>
        /// <para>
        /// <strong>Hash Code Rules:</strong>
        /// <list type="bullet">
        /// <item><description>Equal objects must have equal hash codes</description></item>
        /// <item><description>Hash codes should be distributed evenly across the range of possible values</description></item>
        /// <item><description>The hash code should be stable across multiple calls</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Implementation:</strong> The method uses a standard hash code generation
        /// algorithm that combines the hash codes of the individual properties using prime
        /// number multiplication to ensure good distribution.
        /// </para>
        /// </remarks>
        public override int GetHashCode()
        {
            int hash = 17;
            hash = CompanyId.HasValue ? (hash * 23) + CompanyId.GetHashCode() : hash;
            hash = (hash * 23) + UserId.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Returns a string representation of the UserIdentifier.
        /// </summary>
        /// <returns>
        /// A string representation of the UserIdentifier in the format returned by
        /// <see cref="ToUserIdentifierString"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method provides a human-readable string representation of the UserIdentifier.
        /// It delegates to the <see cref="ToUserIdentifierString"/> method to ensure
        /// consistent formatting.
        /// </para>
        /// <para>
        /// <strong>Usage:</strong> This method is commonly used for debugging, logging,
        /// and displaying user identifiers in user interfaces.
        /// </para>
        /// </remarks>
        public override string ToString()
        {
            return ToUserIdentifierString();
        }

        /// <summary>
        /// Determines whether two UserIdentifier instances are equal.
        /// </summary>
        /// <param name="left">
        /// The first UserIdentifier to compare.
        /// </param>
        /// <param name="right">
        /// The second UserIdentifier to compare.
        /// </param>
        /// <returns>
        /// true if the two UserIdentifier instances are equal; otherwise, false.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This operator provides a convenient way to compare UserIdentifier instances
        /// for equality. It handles null values appropriately and delegates to the
        /// <see cref="Equals(object)"/> method for the actual comparison.
        /// </para>
        /// <para>
        /// <strong>Null Handling:</strong> The operator correctly handles null values,
        /// treating two null references as equal and a null reference as not equal to
        /// any non-null reference.
        /// </para>
        /// </remarks>
        public static bool operator ==(UserIdentifier left, UserIdentifier right)
        {
            if (Equals(left, null))
            {
                return Equals(right, null);
            }

            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two UserIdentifier instances are not equal.
        /// </summary>
        /// <param name="left">
        /// The first UserIdentifier to compare.
        /// </param>
        /// <param name="right">
        /// The second UserIdentifier to compare.
        /// </param>
        /// <returns>
        /// true if the two UserIdentifier instances are not equal; otherwise, false.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This operator provides a convenient way to compare UserIdentifier instances
        /// for inequality. It is the logical negation of the equality operator.
        /// </para>
        /// <para>
        /// <strong>Implementation:</strong> The operator simply returns the negation of
        /// the equality operator result.
        /// </para>
        /// </remarks>
        public static bool operator !=(UserIdentifier left, UserIdentifier right)
        {
            return !(left == right);
        }
    }
}
