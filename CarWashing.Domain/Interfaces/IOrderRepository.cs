using CarWashing.Domain.Filters;
using CarWashing.Domain.Models;

namespace CarWashing.Domain.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders(OrderFilter filter);
    Task<Order?> GetOrder(int id);
    Task<Order> AddOrder(Order order);
    Task UpdateOrder(Order order);
    Task DeleteOrder(int id);
    Task AddServices(int id, List<Service> services);
    Task CompleteOrder(int id);
}