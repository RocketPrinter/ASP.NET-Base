namespace Server;

public static class Extensions
{
    /// <summary>
    /// Appends the item only if it's not found in the enumeration
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumeration"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static IEnumerable<T> AppendDistinct<T>(this IEnumerable<T> enumeration, T item)
    {
        if (enumeration.Contains(item))
            return enumeration;
        else
            return enumeration.Append(item);
    }

    /// <summary>
    /// Used to enumerate through the items of an array from the configuration file
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static IEnumerable<T> GetEnumerable<T>(this IConfiguration configuration, string key)
    {
        var section = configuration.GetSection(key);
        if (!section.Exists())
            return Enumerable.Empty<T>();
        return section.GetChildren().Select(x => configuration.GetValue<T>(x.Key));
    }
}
