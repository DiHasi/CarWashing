namespace CarWashing.Domain.Filters;

public class SortableFieldPathAttribute(string path) : Attribute
{
    public string Path { get; } = path;
    
}

public static class SortableFieldsExtensions
{
    public static string GetPath<TEnum>(this TEnum field) where TEnum : Enum
    {
        var member = typeof(TEnum).GetMember(field.ToString()).First();
        var attribute = (SortableFieldPathAttribute)member.GetCustomAttributes(typeof(SortableFieldPathAttribute), false).First();
        return attribute.Path;
    }
}