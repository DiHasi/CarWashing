using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;

namespace CarWashing.Application.Services;

public class CarService(ICarRepository carRepository)
{
    public async Task<IEnumerable<Car>> GetCars(CarFilter filter)
    {
        return await carRepository.GetCars(filter);
    }

    public async Task<Car?> GetCar(int id)
    {
        return await carRepository.GetCar(id);
    }

    public async Task<Car> AddCar(Car service)
    {
        return await carRepository.AddCar(service);
    }

    public async Task UpdateCar(int id, Car service)
    {
        await carRepository.UpdateCar(id, service);
    }

    public async Task DeleteCar(int id)
    {
        await carRepository.DeleteCar(id);
    }
}