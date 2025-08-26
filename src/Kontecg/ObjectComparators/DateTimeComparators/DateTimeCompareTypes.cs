namespace Kontecg.ObjectComparators.DateTimeComparators
{
    /// <summary>
    /// Specifies the comparison types for DateTime values.
    /// </summary>
    public enum DateTimeCompareTypes
    {
        /// <summary>
        /// Checks if two DateTime values are equal.
        /// </summary>
        Equals,
        /// <summary>
        /// Checks if the first value is less than the second value.
        /// </summary>
        LessThan,
        /// <summary>
        /// Checks if the first value is less than or equal to the second value.
        /// </summary>
        LessOrEqualThan,
        /// <summary>
        /// Checks if the first value is greater than the second value.
        /// </summary>
        BiggerThan,
        /// <summary>
        /// Checks if the first value is greater than or equal to the second value.
        /// </summary>
        BiggerOrEqualThan
    }

    /// <summary>
    /// Specifies the comparison types for nullable DateTime values.
    /// </summary>
    public enum NullableDateTimeCompareTypes
    {
        /// <summary>
        /// Checks if the value is null.
        /// </summary>
        Null,
        /// <summary>
        /// Checks if the value is not null.
        /// </summary>
        NotNull,
        /// <summary>
        /// Checks if two nullable DateTime values are equal.
        /// </summary>
        Equals,
        /// <summary>
        /// Checks if the first value is less than the second value.
        /// </summary>
        LessThan,
        /// <summary>
        /// Checks if the first value is less than or equal to the second value.
        /// </summary>
        LessOrEqualThan,
        /// <summary>
        /// Checks if the first value is greater than the second value.
        /// </summary>
        BiggerThan,
        /// <summary>
        /// Checks if the first value is greater than or equal to the second value.
        /// </summary>
        BiggerOrEqualThan
    }
}
