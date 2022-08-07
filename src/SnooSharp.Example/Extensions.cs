namespace SnooSharp.Example;

public static class Extensions
{
    public static bool IsSingle<T>(this IEnumerable<T?> me, out T? value)
    {
        value = default;
        var enumerable = me as T?[] ?? me.ToArray();
        var g = enumerable.Any() && !enumerable.Skip(1).Any();
        if (g) value = enumerable.Single();
        return g;
    }
}