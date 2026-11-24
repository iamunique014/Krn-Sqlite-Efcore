using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace KasiRoomNetwork.Common.Models.Enums;

/// <summary>
/// Converts PropertyType enum values and their persisted names into user-facing labels.
/// Persisted values remain enum names for backwards compatibility.
/// </summary>
public static class PropertyTypeDisplay
{
    public static string GetDisplayName(PropertyType? propertyType)
    {
        if (!propertyType.HasValue)
        {
            return string.Empty;
        }

        var member = typeof(PropertyType).GetMember(propertyType.Value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName()
            ?? SplitPascalCase(propertyType.Value.ToString());
    }

    public static string GetDisplayName(string? storedValue)
    {
        if (string.IsNullOrWhiteSpace(storedValue))
        {
            return string.Empty;
        }

        var value = storedValue.Trim();
        return Enum.TryParse<PropertyType>(value, ignoreCase: true, out var propertyType)
            && Enum.IsDefined(propertyType)
                ? GetDisplayName(propertyType)
                : SplitPascalCase(value);
    }

    private static string SplitPascalCase(string value) =>
        Regex.Replace(value, "(?<=[a-z0-9])([A-Z])", " $1");
}
