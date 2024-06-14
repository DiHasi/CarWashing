using System.Text.Json.Serialization;
using AutoFilter;
using AutoFilter.Filters;

namespace CarWashing.Domain.Filters;

public class CarFilter : Filter
{
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Model { get; set; }
    [NavigationProperty("Brand", TargetPropertyName = "Name", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Brand { get; set; }
    
    public CarSortableFields? OrderBy { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CarSortableFields
{
    [SortableFieldPath("Model")]
    Model,
    [SortableFieldPath("Brand")]
    Brand
}
