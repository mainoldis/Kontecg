using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Kontecg.Baseline
{
    internal static class CollectionExtensions
    {
        public static IList<T> RemoveAll<T>([NotNull] this ICollection<T> source, Func<T, bool> predicate)
        {
            List<T> items = source.Where(predicate).ToList();

            foreach (T item in items)
            {
                source.Remove(item);
            }

            return items;
        }
    }
}
