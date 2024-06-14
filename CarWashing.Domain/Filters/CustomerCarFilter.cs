using System.Text.Json.Serialization;
using AutoFilter;
using AutoFilter.Filters;

namespace CarWashing.Domain.Filters;

public class CustomerCarFilter : Filter
{
    [NavigationProperty("Car", TargetPropertyName = "Model", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Model { get; set; }
    [NavigationProperty("Car", TargetPropertyName = "Brand", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? Brand { get; set; }
    [NavigationProperty("Customer", TargetPropertyName = "FirstName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerFirstName { get; set; }
    [NavigationProperty("Customer", TargetPropertyName = "LastName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerLastName { get; set; }
    [NavigationProperty("Customer", TargetPropertyName = "Patronymic", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerPatronymic { get; set; }
    [NavigationProperty("Customer", TargetPropertyName = "Email", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerEmail { get; set; }
    public int? Year { get; set; }
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public string? Number { get; set; }
    
    public CustomerCarSortableFields? OrderBy { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CustomerCarSortableFields
{
    [SortableFieldPath("FirstName")]
    FirstName,
    [SortableFieldPath("LastName")]
    LastName,
    [SortableFieldPath("Patronymic")]
    Patronymic,
    [SortableFieldPath("Email")]
    Email,
    [SortableFieldPath("Model")]
    Model,
    [SortableFieldPath("Brand")]
    Brand,
    [SortableFieldPath("Year")]
    Year,
    [SortableFieldPath("Number")]  
    Number
    
}