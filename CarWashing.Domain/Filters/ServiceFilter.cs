using AutoFilter;
using AutoFilter.Filters;

namespace CarWashing.Domain.Filters;

public class ServiceFilter : Filter
{
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Search { get; set; }
    [NavigationProperty("Price", TargetPropertyName = "MaxValue")] 
    public Range<int>? Price { get; set; }
    [NavigationProperty("Time", TargetPropertyName = "Minutes")] 
    public Range<int>? Time { get; set; }
    
    
}