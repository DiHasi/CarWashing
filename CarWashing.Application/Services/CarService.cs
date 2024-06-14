using CarWashing.Domain.Filters;
using CarWashing.Domain.Interfaces;
using CarWashing.Domain.Models;
using CSharpFunctionalExtensions;

namespace CarWashing.Application.Services;

public class CarService(ICarRepository carRepository, IBrandRepository brandRepository)
{
    public async Task<IEnumerable<Car>> GetCars(CarFilter filter)
    {
        return await carRepository.GetCars(filter);
    }

    public async Task<Car?> GetCar(int id)
    {
        return await carRepository.GetCar(id);
    }

    public async Task<Result<Car>> AddCar(string model, int brandId)
    {
        var brand = await brandRepository.GetBrand(brandId);
        if (brand == null) return Result.Failure<Car>("Brand not found");

        var result = Car.Create(model, brand);
        if(result.IsFailure) return Result.Failure<Car>(result.Error);
        
        return await carRepository.AddCar(result.Value);
    }

    public async Task<Result<Car>> UpdateCar(int id, string model, int brandId)
    {
        var brand = await brandRepository.GetBrand(brandId);
        if (brand == null) return Result.Failure<Car>("Brand not found");
        
        var carToUpdate = await GetCar(id);
        if (carToUpdate == null) return Result.Failure<Car>("Car not found");
        
        var result = carToUpdate.ChangeModel(model);
        if (result.IsFailure) return Result.Failure<Car>(result.Error);

        result = result.Value.ChangeBrand(brand);
        if (result.IsFailure) return Result.Failure<Car>(result.Error);

        await carRepository.UpdateCar(result.Value);

        return Result.Success(result.Value);
    }

    public async Task DeleteCar(int id)
    {
        await carRepository.DeleteCar(id);
    }
}