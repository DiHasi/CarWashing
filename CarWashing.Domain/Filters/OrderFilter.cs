using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AutoFilter;
using AutoFilter.Filters;
using CarWashing.Domain.Enums;

namespace CarWashing.Domain.Filters;

public class OrderFilter : Filter
{
    [NavigationProperty("Administrator", TargetPropertyName = "FirstName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? AdministratorFirstName { get; set; }
    [NavigationProperty("Administrator", TargetPropertyName = "LastName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? AdministratorLastName { get; set; }
    [NavigationProperty("Administrator", TargetPropertyName = "Patronymic", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? AdministratorPatronymic { get; set; }
    [NavigationProperty("Employee", TargetPropertyName = "FirstName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? EmployeeFirstName { get; set; }
    [NavigationProperty("Employee", TargetPropertyName = "LastName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? EmployeeLastName { get; set; }
    [NavigationProperty("Employee", TargetPropertyName = "Patronymic", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? EmployeePatronymic { get; set; }
    
    [NavigationProperty("CustomerCar.Customer", TargetPropertyName = "FirstName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerFirstName { get; set; }
    [NavigationProperty("CustomerCar.Customer", TargetPropertyName = "LastName", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerLastName { get; set; }
    [NavigationProperty("CustomerCar.Customer", TargetPropertyName = "Patronymic", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerPatronymic { get; set; }
    [NavigationProperty("CustomerCar.Customer", TargetPropertyName = "Email", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CustomerEmail { get; set; }
    [NavigationProperty("CustomerCar.Car", TargetPropertyName = "Model", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CarBrand { get; set; }
    [NavigationProperty("CustomerCar.Car", TargetPropertyName = "Brand", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? CarModel { get; set; }
    
    [NavigationProperty("CustomerCar", TargetPropertyName = "Year")] 
    public int? Year { get; set; }
    [NavigationProperty("CustomerCar", TargetPropertyName = "Number", 
        StringFilter = StringFilterCondition.Contains, IgnoreCase = true)] 
    public string? Number { get; set; }
    [FilterProperty(StringFilter = StringFilterCondition.Contains, IgnoreCase = true)]
    public Status? Status { get; set; }
    public Range<DateTime>? Date { get; set; }
    
    [NotAutoFiltered]
    public OrderSortableFields? OrderBy { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderSortableFields
{
    [SortableFieldPath("Administrator.FirstName")]
    AdministratorFirstName,
    [SortableFieldPath("Administrator.LastName")]
    AdministratorLastName,
    [SortableFieldPath("Administrator.Patronymic")]
    AdministratorPatronymic,
    [SortableFieldPath("Employee.FirstName")]
    EmployeeFirstName,
    [SortableFieldPath("Employee.LastName")]
    EmployeeLastName,
    [SortableFieldPath("Employee.Patronymic")]
    EmployeePatronymic,
    [SortableFieldPath("CustomerCar.Customer.FirstName")]
    CustomerFirstName,
    [SortableFieldPath("CustomerCar.Customer.LastName")]
    CustomerLastName,
    [SortableFieldPath("CustomerCar.Customer.Patronymic")]
    CustomerPatronymic,
    [SortableFieldPath("CustomerCar.Customer.Email")]
    CustomerEmail,
    [SortableFieldPath("CustomerCar.Car.Brand.Name")]
    CarBrand,
    [SortableFieldPath("CustomerCar.Car.Model")]
    CarModel,
    [SortableFieldPath("CustomerCar.Year")]
    Year,
    [SortableFieldPath("CustomerCar.Number")]
    Number,
    [SortableFieldPath("Status")]
    Status,
    [SortableFieldPath("StartDate")]
    StartDate,
    [SortableFieldPath("EndDate")]
    EndDate
}