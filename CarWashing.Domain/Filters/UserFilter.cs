using System.Text.Json.Serialization;
using AutoFilter;

namespace CarWashing.Domain.Filters;

public class UserFilter : Filter
{
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? FirstName { get; set; }
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? LastName { get; set; }
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Patronymic { get; set; }
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Email { get; set; }
    public bool? IsSendNotify { get; set; }
    
    [NotAutoFiltered]
    public UserSortableFields? OrderBy { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserSortableFields
{
    [SortableFieldPath("FirstName")]
    FirstName,
    [SortableFieldPath("LastName")]
    LastName,
    [SortableFieldPath("Patronymic")]
    Patronymic,
    [SortableFieldPath("Email")]
    Email,
    [SortableFieldPath("IsSendNotify")]
    IsSendNotify
}