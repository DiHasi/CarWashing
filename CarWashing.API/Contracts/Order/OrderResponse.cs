using CarWashing.Contracts.CustomerCar;
using CarWashing.Contracts.Service;
using CarWashing.Domain.Enums;

namespace CarWashing.Contracts.Order;

public record UserOrderResponse(
    int Id,
    string FullName);

public record OrderResponse(
    int Id,
    Status Status,
    string StartDate,
    string EndDate,
    int TotalPrice,
    int TotalTime,
    UserOrderResponse Administrator,
    UserOrderResponse Employee,
    CustomerCarResponse CustomerCar,
    List<ServiceResponse> Services);