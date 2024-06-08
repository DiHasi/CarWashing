using CarWashing.Domain.Filters;
using CarWashing.Domain.Models;

namespace CarWashing.Domain.Interfaces;

public interface ICarRepository
{
    Task<IEnumerable<Car>> GetCars(CarFilter filter);
    Task<Car?> GetCar(int id);
    Task<Car> AddCar(Car car);
    Task UpdateCar(int id, Car service);
    Task DeleteCar(int id);
}