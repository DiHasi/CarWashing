using CarWashing.Contracts.CustomerCar;
using CarWashing.Contracts.Service;

namespace CarWashing.Contracts.Order;

public record UserOrderResponse(
    int Id,
    string FullName);

public record OrderResponse(
    int Id,
    int Status,
    string StartDate,
    string EndDate,
    int TotalPrice,
    int TotalTime,
    UserOrderResponse Administrator,
    UserOrderResponse Employee,
    CustomerCarResponse CustomerCar,
    List<ServiceResponse> Services);