using CarWashing.Application.Services;
using CarWashing.Contracts.Car;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using Microsoft.AspNetCore.Mvc;
using CarWashing.Domain.Models;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Role.Administrator))]
public class CarController(CarService carService, BrandService brandService) : ControllerBase
{
    // GET: api/Car
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CarResponse>>> GetCars([FromQuery]CarFilter filter)
    {
        var cars = await carService.GetCars(filter);
        var carResponses = cars
            .Select(car => new CarResponse(car.Id, car.Model, car.Brand.Name));
        return Ok(carResponses);
    }

    // GET: api/Car/5
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<CarResponse>> GetCar(int id)
    {
        var car = await carService.GetCar(id);
        
        if (car == null)
        {
            return NotFound();
        }
        
        return new CarResponse(car.Id, car.Model, car.Brand.Name);
    }

    // PUT: api/Car/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutCar(int id, CarRequest request)
    {
        var result = await carService.UpdateCar(id, request.Model, request.BrandId);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok("Updated");
    }

    // POST: api/Car
    [HttpPost]
    public async Task<ActionResult<CarEntity>> PostCar(CarRequest request)
    {
        var result = await carService.AddCar(request.Model, request.BrandId);
        if(result.IsFailure) return BadRequest(result.Error);

        var response = new CarResponse(result.Value.Id, result.Value.Model, result.Value.Brand.Name);
        return CreatedAtAction("GetCar", new { id = result.Value.Id }, response);
    }

    // DELETE: api/Car/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCar(int id)
    {
        var car = await carService.GetCar(id);
        if (car == null)
        {
            return NotFound();
        }
        
        await carService.DeleteCar(id);
        return Ok("Deleted");
    }
}