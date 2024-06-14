using AutoFilter;

namespace CarWashing.Domain.Filters;

public class Filter
{
    [NotAutoFiltered] public bool ByDescending { get; set; } = false;
    [NotAutoFiltered] public int PageSize { get; set; } = 10;

    [NotAutoFiltered] public int PageNumber { get; set; } = 1;
}