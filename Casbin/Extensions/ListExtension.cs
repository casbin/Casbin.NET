using System.Collections.Generic;

namespace Casbin;

internal static class ListExtension
{
    internal static bool TryGetValue<T>(this IReadOnlyList<T> list, int index, out T value)
    {
        if (index < 0 || index >= list.Count)
        {
            value = default;
            return false;
        }

        value = list[index];
        return true;
    }

    internal static T GetValueOrDefault<T>(this IReadOnlyList<T> list, int index) =>
        list.TryGetValue(index, out T value) ? value : default;
}
