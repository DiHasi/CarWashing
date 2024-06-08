using AutoFilter;
using AutoFilter.Filters;

namespace CarWashing.Domain.Filters;

public class BrandFilter : Filter
{
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Search { get; set; }
}