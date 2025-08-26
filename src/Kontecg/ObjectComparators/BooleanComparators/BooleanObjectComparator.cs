using System;

namespace Kontecg.ObjectComparators.BooleanComparators
{
    /// <summary>
    /// Compares boolean values using specified comparison types.
    /// </summary>
    public class BooleanObjectComparator : ObjectComparatorBase<bool, BooleanCompareTypes>
    {
        /// <summary>
        /// Compares two boolean values using the specified comparison type.
        /// </summary>
        /// <param name="baseObject">The base boolean value.</param>
        /// <param name="compareObject">The boolean value to compare.</param>
        /// <param name="compareType">The type of comparison to perform.</param>
        /// <returns>True if the comparison condition is met; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the comparison type is not supported.</exception>
        protected override bool Compare(bool baseObject, bool compareObject, BooleanCompareTypes compareType)
        {
            switch (compareType)
            {
                case BooleanCompareTypes.Equals:
                    return baseObject == compareObject;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }

    /// <summary>
    /// Compares nullable boolean values using specified comparison types.
    /// </summary>
    public class NullableBooleanObjectComparator : ObjectComparatorBase<bool?, NullableBooleanCompareTypes>
    {
        /// <summary>
        /// Compares two nullable boolean values using the specified comparison type.
        /// </summary>
        /// <param name="baseObject">The base nullable boolean value.</param>
        /// <param name="compareObject">The nullable boolean value to compare.</param>
        /// <param name="compareType">The type of comparison to perform.</param>
        /// <returns>True if the comparison condition is met; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the comparison type is not supported.</exception>
        protected override bool Compare(bool? baseObject, bool? compareObject, NullableBooleanCompareTypes compareType)
        {
            switch (compareType)
            {
                case NullableBooleanCompareTypes.Equals:
                    return baseObject == compareObject;
                case NullableBooleanCompareTypes.Null:
                    return !baseObject.HasValue;
                case NullableBooleanCompareTypes.NotNull:
                    return baseObject.HasValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }
}
