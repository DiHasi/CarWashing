using System.Text.Json.Serialization;
using AutoFilter;
using AutoFilter.Filters;

namespace CarWashing.Domain.Filters;

public class BrandFilter : Filter
{
    [FilterProperty(TargetPropertyName = "Name", StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Search { get; set; }
    
    public BrandSortableFields? OrderBy { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BrandSortableFields
{
    [SortableFieldPath("Name")]
    Name
}