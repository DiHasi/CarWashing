using CarWashing.Application.Services;
using CarWashing.Contracts.Car;
using CarWashing.Contracts.CustomerCar;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Role.Administrator))]
public class CustomerCarController(CustomerCarService customerCarService) : ControllerBase
{
    // GET: api/CustomerCar
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerCarResponse>>> GetCustomerCars(
        [FromQuery] CustomerCarFilter filter)
    {
        var customerCars = await customerCarService.GetCustomerCars(filter);

        var customerCarResponses = customerCars
            .Select(c => new CustomerCarResponse(c.Id,
                new CarResponse(c.Car.Id, c.Car.Model, c.Car.Brand.Name),
                c.Customer.FullName, c.Customer.Email, c.Year,
                c.Number));

        return Ok(customerCarResponses);
    }

    // GET: api/CustomerCar/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerCarResponse>> GetCustomerCar(int id)
    {
        var customerCar = await customerCarService.GetCustomerCar(id);

        if (customerCar == null)
        {
            return NotFound();
        }

        var carResponse = new CarResponse(customerCar.Car.Id, customerCar.Car.Model, customerCar.Car.Brand.Name);

        return new CustomerCarResponse(customerCar.Id, carResponse, customerCar.Customer.FullName,
            customerCar.Customer.Email, customerCar.Year,
            customerCar.Number);
    }

    // PUT: api/CustomerCar/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutCustomerCar(int id, CustomerCarRequest request)
    {
        var result =
            await customerCarService.UpdateCustomerCar(id, request.CarId, request.CustomerId, request.Year,
                request.Number);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok("Updated");
    }

    // POST: api/CustomerCar
    [HttpPost]
    public async Task<ActionResult<CustomerCarResponse>> PostCustomerCar(CustomerCarRequest request)
    {
        var result = await customerCarService.AddCustomerCar(request.CarId, request.CustomerId, request.Year,
            request.Number);
        if (result.IsFailure) return BadRequest(result.Error);

        var carResponse = new CarResponse(result.Value.Car.Id, result.Value.Car.Model, result.Value.Car.Brand.Name);

        var response = new CustomerCarResponse(result.Value.Id, carResponse, result.Value.Customer.FullName,
            result.Value.Customer.Email,
            result.Value.Year, result.Value.Number);

        return CreatedAtAction("GetCustomerCars", new { id = result.Value.Id }, response);
    }

    // DELETE: api/CustomerCar/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCustomerCar(int id)
    {
        var service = await customerCarService.GetCustomerCar(id);
        if (service == null)
        {
            return NotFound();
        }

        await customerCarService.DeleteCustomerCar(id);

        return Ok("Deleted");
    }
}