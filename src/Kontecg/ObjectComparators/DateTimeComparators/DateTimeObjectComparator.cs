using System;

namespace Kontecg.ObjectComparators.DateTimeComparators
{
    /// <summary>
    /// Compares DateTime values using specified comparison types.
    /// </summary>
    public class DateTimeObjectComparator : ObjectComparatorBase<DateTime, DateTimeCompareTypes>
    {
        /// <summary>
        /// Compares two DateTime values using the specified comparison type.
        /// </summary>
        /// <param name="baseObject">The base DateTime value.</param>
        /// <param name="compareObject">The DateTime value to compare.</param>
        /// <param name="compareType">The type of comparison to perform.</param>
        /// <returns>True if the comparison condition is met; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the comparison type is not supported.</exception>
        protected override bool Compare(DateTime baseObject, DateTime compareObject, DateTimeCompareTypes compareType)
        {
            switch (compareType)
            {
                case DateTimeCompareTypes.Equals:
                    return baseObject.Equals(compareObject);
                case DateTimeCompareTypes.LessThan:
                    return baseObject.CompareTo(compareObject) < 0;
                case DateTimeCompareTypes.LessOrEqualThan:
                    return baseObject.CompareTo(compareObject) <= 0;
                case DateTimeCompareTypes.BiggerThan:
                    return baseObject.CompareTo(compareObject) > 0;
                case DateTimeCompareTypes.BiggerOrEqualThan:
                    return baseObject.CompareTo(compareObject) >= 0;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }

    /// <summary>
    /// Compares nullable DateTime values using specified comparison types.
    /// </summary>
    public class NullableDateTimeObjectComparator : ObjectComparatorBase<DateTime?, NullableDateTimeCompareTypes>
    {
        /// <summary>
        /// Compares two nullable DateTime values using the specified comparison type.
        /// </summary>
        /// <param name="baseObject">The base nullable DateTime value.</param>
        /// <param name="compareObject">The nullable DateTime value to compare.</param>
        /// <param name="compareType">The type of comparison to perform.</param>
        /// <returns>True if the comparison condition is met; otherwise, false.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the comparison type is not supported.</exception>
        protected override bool Compare(DateTime? baseObject, DateTime? compareObject,
            NullableDateTimeCompareTypes compareType)
        {
            bool conditionBothHasValue = baseObject.HasValue && compareObject.HasValue;
            switch (compareType)
            {
                case NullableDateTimeCompareTypes.Equals:
                    return baseObject.Equals(compareObject);
                case NullableDateTimeCompareTypes.LessThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) < 0;
                case NullableDateTimeCompareTypes.LessOrEqualThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) <= 0;
                case NullableDateTimeCompareTypes.BiggerThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) > 0;
                case NullableDateTimeCompareTypes.BiggerOrEqualThan:
                    return conditionBothHasValue && baseObject.Value.CompareTo(compareObject.Value) >= 0;
                case NullableDateTimeCompareTypes.Null:
                    return !baseObject.HasValue;
                case NullableDateTimeCompareTypes.NotNull:
                    return baseObject.HasValue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
            }
        }
    }
}
