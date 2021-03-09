using System.Collections.Generic;

namespace Casbin.UnitTests.Extensions
{
    internal static class EnumerableExtension
    {
        #region DeepEquals

        internal static bool DeepEquals<T>(this IEnumerable<T> enumerable, IEnumerable<T> anotherEnumerable)
        {
            if (enumerable is null || anotherEnumerable is null)
            {
                return false;
            }

            if (enumerable is T[] array && anotherEnumerable is T[] anotherArray)
            {
                return array.DeepEquals(anotherArray);
            }

            if (enumerable is IReadOnlyList<T> list && anotherEnumerable is IReadOnlyList<T> anotherList)
            {
                return list.DeepEquals(anotherList);
            }

            using var enumerator = enumerable.GetEnumerator();
            using var anotherEnumerator = anotherEnumerable.GetEnumerator();
            while (enumerator.MoveNext() && anotherEnumerator.MoveNext())
            {
                if (enumerator.Current is null || anotherEnumerator.Current is null)
                {
                    return false;
                }

                if (enumerator.Current.Equals(anotherEnumerator.Current) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this T[] array, T[] anotherArray)
        {
            if (array is null || anotherArray is null)
            {
                return false;
            }

            int length = array.Length;
            if (length != anotherArray.Length)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (array[index].Equals(anotherArray[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IReadOnlyList<T> list, IReadOnlyList<T> anotherList)
        {
            if (list is null || anotherList is null)
            {
                return false;
            }

            int length = list.Count;
            if (length != anotherList.Count)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (list[index].Equals(anotherList[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IEnumerable<IEnumerable<T>> enumerable, IEnumerable<IEnumerable<T>> anotherEnumerable)
        {
            if (enumerable is null || anotherEnumerable is null)
            {
                return false;
            }

            if (enumerable is IEnumerable<T>[] array && anotherEnumerable is IEnumerable<T>[] anotherArray)
            {
                return array.DeepEquals(anotherArray);
            }

            if (enumerable is IReadOnlyList<IEnumerable<T>> list && anotherEnumerable is IReadOnlyList<IEnumerable<T>> anotherList)
            {
                return list.DeepEquals(anotherList);
            }

            using var enumerator = enumerable.GetEnumerator();
            using var anotherEnumerator = anotherEnumerable.GetEnumerator();
            while (enumerator.MoveNext() && anotherEnumerator.MoveNext())
            {
                if (enumerator.Current is null || anotherEnumerator.Current is null)
                {
                    return false;
                }

                if (enumerator.Current.DeepEquals(anotherEnumerator.Current) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IEnumerable<T>[] array, IEnumerable<T>[] anotherArray)
        {
            if (array is null || anotherArray is null)
            {
                return false;
            }

            int length = array.Length;
            if (length != anotherArray.Length)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (array[index].DeepEquals(anotherArray[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool DeepEquals<T>(this IReadOnlyList<IEnumerable<T>> list, IReadOnlyList<IEnumerable<T>> anotherList)
        {
            if (list is null || anotherList is null)
            {
                return false;
            }

            int length = list.Count;
            if (length != anotherList.Count)
            {
                return false;
            }

            for (int index = 0; index < length; index++)
            {
                if (list[index].DeepEquals(anotherList[index]) is false)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
