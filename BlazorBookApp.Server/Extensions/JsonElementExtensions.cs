namespace BlazorBookApp.Server.Extensions;

/// <summary>
/// JsonElement helpers.
/// </summary>
internal static class JsonElementExtensions
{
    /// <summary>
    /// Safely get a property or null.
    /// </summary>
    public static JsonElement? GetPropertyOrNull(this JsonElement el, string name)
    {
        if (el.ValueKind != JsonValueKind.Object) return null;
        if (el.TryGetProperty(name, out var prop)) return prop;
        return null;
    }
}
