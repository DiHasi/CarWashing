using System.Text.Json.Serialization;
using AutoFilter;
using AutoFilter.Filters;

namespace CarWashing.Domain.Filters;

public class ServiceFilter : Filter
{
    [FilterProperty(TargetPropertyName = "Name", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? Search { get; set; }
    [NavigationProperty("Price", TargetPropertyName = "MaxValue")] 
    public Range<int>? Price { get; set; }
    [NavigationProperty("Time", TargetPropertyName = "Minutes")] 
    public Range<int>? Time { get; set; }
    
    public ServiceSortableFields? OrderBy{ get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServiceSortableFields
{
    [SortableFieldPath("Name")]
    Name,
    [SortableFieldPath("Price.MaxValue")]
    Price,
    [SortableFieldPath("Time.Minutes")]
    Time
}