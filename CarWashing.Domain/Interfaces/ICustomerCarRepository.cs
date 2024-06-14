using CarWashing.Domain.Filters;
using CarWashing.Domain.Models;

namespace CarWashing.Domain.Interfaces;

public interface ICustomerCarRepository
{
    Task<IEnumerable<CustomerCar>> GetCustomerCars(CustomerCarFilter filter);
    Task<CustomerCar?> GetCustomerCar(int id);
    Task<CustomerCar> AddCustomerCar(CustomerCar customerCar);
    Task UpdateCustomerCar(CustomerCar customerCar);
    Task DeleteCustomerCar(int id);
}