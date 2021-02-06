using System;
using System.Collections.Generic;
using System.Linq;

namespace NovgorodBot.Services.Extensions
{
    public static class EnumerableExtensions
    {
        public static string JoinToString<T>(this IEnumerable<T> source, string separator = ", ")
        {
            if (source == null)
                return null;

            var list = source.ToList();

            var result = string.Join(separator, list);

            return result;
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }
    }
}
