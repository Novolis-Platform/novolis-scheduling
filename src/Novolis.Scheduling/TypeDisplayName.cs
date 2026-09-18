namespace Novolis.Scheduling;

internal static class TypeDisplayName
{
    public static string GetFullDisplayName(this Type type) => type.FullName ?? type.Name;
}
