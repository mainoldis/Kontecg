using System;
using System.Collections.Generic;
using System.Linq;
using Kontecg.Collections.Extensions;

namespace Kontecg
{
    /// <summary>
    /// Provides utility methods for random number generation and collection randomization operations.
    /// This class serves as a thread-safe wrapper around the <see cref="Random"/> class with additional
    /// convenience methods for common randomization tasks.
    /// </summary>
    /// <remarks>
    /// <para>
    /// RandomHelper provides a centralized, thread-safe approach to random number generation
    /// throughout the Kontecg framework. It includes:
    /// </para>
    /// <list type="bullet">
    /// <item><description>Thread-safe random number generation with proper locking</description></item>
    /// <item><description>Range-based random number generation</description></item>
    /// <item><description>Random selection from collections</description></item>
    /// <item><description>Collection randomization and shuffling</description></item>
    /// </list>
    /// <para>
    /// All methods use a single, shared <see cref="Random"/> instance with proper synchronization
    /// to ensure thread safety and consistent random number generation across the application.
    /// </para>
    /// <para>
    /// <strong>Thread Safety:</strong> All methods in this class are thread-safe and can be called
    /// concurrently from multiple threads without additional synchronization.
    /// </para>
    /// </remarks>
    public static class RandomHelper
    {
        /// <summary>
        /// The shared random number generator instance used by all methods in this class.
        /// </summary>
        /// <remarks>
        /// This field is marked as readonly to ensure the same Random instance is used
        /// throughout the application lifetime. All access to this field is synchronized
        /// using lock statements to ensure thread safety.
        /// </remarks>
        private static readonly Random Rnd = new();

        /// <summary>
        /// Returns a random number within a specified range.
        /// </summary>
        /// <param name="minValue">
        /// The inclusive lower bound of the random number returned.
        /// </param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number returned. maxValue must be greater than or equal
        /// to minValue.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to minValue and less than maxValue;
        /// that is, the range of return values includes minValue but not maxValue.
        /// If minValue equals maxValue, minValue is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when maxValue is less than minValue.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method generates a random integer within the specified range [minValue, maxValue).
        /// The method is thread-safe and uses proper synchronization to ensure consistent
        /// random number generation.
        /// </para>
        /// <para>
        /// <strong>Range Behavior:</strong> The returned value is inclusive of minValue but
        /// exclusive of maxValue. If minValue equals maxValue, minValue is returned.
        /// </para>
        /// </remarks>
        public static int GetRandom(int minValue, int maxValue)
        {
            lock (Rnd)
            {
                return Rnd.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. maxValue must be greater than or
        /// equal to zero.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue;
        /// that is, the range of return values ordinarily includes zero but not maxValue.
        /// However, if maxValue equals zero, maxValue is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when maxValue is less than zero.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method generates a random integer in the range [0, maxValue). It is equivalent
        /// to calling <see cref="GetRandom(int, int)"/> with minValue = 0.
        /// </para>
        /// <para>
        /// <strong>Special Case:</strong> If maxValue is zero, the method returns zero.
        /// </para>
        /// </remarks>
        public static int GetRandom(int maxValue)
        {
            lock (Rnd)
            {
                return Rnd.Next(maxValue);
            }
        }

        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero and less than
        /// <see cref="int.MaxValue"/>.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method generates a random nonnegative integer in the range [0, int.MaxValue).
        /// It is equivalent to calling <see cref="GetRandom(int)"/> with maxValue = int.MaxValue.
        /// </para>
        /// <para>
        /// <strong>Note:</strong> The returned value will never be int.MaxValue due to the
        /// exclusive upper bound behavior of the underlying Random.Next() method.
        /// </para>
        /// </remarks>
        public static int GetRandom()
        {
            lock (Rnd)
            {
                return Rnd.Next();
            }
        }

        /// <summary>
        /// Gets a random element from the provided array of objects.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the objects in the array.
        /// </typeparam>
        /// <param name="objs">
        /// The array of objects from which to select a random element.
        /// </param>
        /// <returns>
        /// A randomly selected element from the provided array.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when the objs parameter is null or empty.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method randomly selects one element from the provided array using uniform
        /// distribution. Each element has an equal probability of being selected.
        /// </para>
        /// <para>
        /// <strong>Thread Safety:</strong> This method is thread-safe and can be called
        /// concurrently from multiple threads.
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> The method has O(1) time complexity for selection,
        /// making it efficient for arrays of any size.
        /// </para>
        /// </remarks>
        public static T GetRandomOf<T>(params T[] objs)
        {
            if (objs.IsNullOrEmpty())
            {
                throw new ArgumentException("objs can not be null or empty!", nameof(objs));
            }

            return objs[GetRandom(0, objs.Length)];
        }

        /// <summary>
        /// Generates a randomized (shuffled) list from the given enumerable collection.
        /// </summary>
        /// <typeparam name="T">
        /// The type of items in the collection.
        /// </typeparam>
        /// <param name="items">
        /// The enumerable collection of items to randomize.
        /// </param>
        /// <returns>
        /// A new <see cref="List{T}"/> containing all items from the original collection
        /// in a randomized order.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when the items parameter is null.
        /// </exception>
        /// <remarks>
        /// <para>
        /// This method creates a completely randomized version of the input collection using
        /// the Fisher-Yates shuffle algorithm. The original collection remains unchanged,
        /// and a new list is returned with the items in random order.
        /// </para>
        /// <para>
        /// <strong>Algorithm:</strong> The method uses an in-place shuffle algorithm that:
        /// <list type="bullet">
        /// <item><description>Creates a copy of the input collection</description></item>
        /// <item><description>Iteratively selects random elements and removes them</description></item>
        /// <item><description>Builds a new list with the randomly selected elements</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// <strong>Performance:</strong> The method has O(n) time complexity where n is the
        /// number of items in the collection. Memory usage is O(n) for the new list.
        /// </para>
        /// <para>
        /// <strong>Thread Safety:</strong> This method is thread-safe and can be called
        /// concurrently from multiple threads.
        /// </para>
        /// </remarks>
        public static List<T> GenerateRandomizedList<T>(IEnumerable<T> items)
        {
            List<T> currentList = new List<T>(items);
            List<T> randomList = new List<T>();

            while (currentList.Any())
            {
                int randomIndex = GetRandom(0, currentList.Count);
                randomList.Add(currentList[randomIndex]);
                currentList.RemoveAt(randomIndex);
            }

            return randomList;
        }
    }
}
