namespace Kontecg.ObjectComparators.BooleanComparators
{
    /// <summary>
    /// Specifies the comparison types for boolean values.
    /// </summary>
    public enum BooleanCompareTypes
    {
        /// <summary>
        /// Checks if two boolean values are equal.
        /// </summary>
        Equals
    }

    /// <summary>
    /// Specifies the comparison types for nullable boolean values.
    /// </summary>
    public enum NullableBooleanCompareTypes
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
        /// Checks if two nullable boolean values are equal.
        /// </summary>
        Equals
    }
}
