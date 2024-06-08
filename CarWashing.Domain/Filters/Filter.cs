using AutoFilter;

namespace CarWashing.Domain.Filters;

public class Filter
{
    [NotAutoFiltered] public string? OrderBy { get; set; }
    [NotAutoFiltered] public int PageSize { get; set; } = 10;

    [NotAutoFiltered] public int PageNumber { get; set; } = 1;
}