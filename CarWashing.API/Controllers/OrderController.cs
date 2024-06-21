using CarWashing.Application.Services;
using CarWashing.Contracts.Car;
using CarWashing.Contracts.CustomerCar;
using CarWashing.Contracts.Order;
using CarWashing.Contracts.Service;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Role.Administrator)+","+nameof(Role.Employee)+","+nameof(Role.User))]
public class OrderController(OrderService orderService) : ControllerBase
{
    // GET: api/Order
    [HttpGet]
    [Authorize(Roles = nameof(Role.User))]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetOrders(
        [FromQuery] OrderFilter filter)
    {
        var orders = await orderService.GetOrders(filter, User);

        var ordersList = orders.Value.ToList();

        var orderResponses = new List<OrderResponse>();
        foreach (var order in ordersList)
        {
            var administrator = new UserOrderResponse(order.Administrator.Id, order.Administrator.FullName);
            var employee = new UserOrderResponse(order.Employee.Id, order.Employee.FullName);
            var car = new CarResponse(order.CustomerCar.Car.Id, order.CustomerCar.Car.Model,
                order.CustomerCar.Car.Brand.Name);
            var customerCar = new CustomerCarResponse(order.CustomerCar.Id, car, order.CustomerCar.Customer.FullName,
                order.CustomerCar.Customer.Email, order.CustomerCar.Year, order.CustomerCar.Number);
            var services = order.Services.Select(x => new ServiceResponse(x.Id, x.Name, x.Price, x.Time)).ToList();
            orderResponses.Add(new OrderResponse(order.Id, order.Status, order.StartDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                order.EndDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"), order.TotalPrice, order.TotalTime, administrator, employee,
                customerCar, services));
        }

        return Ok(orderResponses);
    }

    // GET: api/Order/5
    [Authorize(Roles = nameof(Role.Administrator)+","+nameof(Role.Employee))]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(int id)
    {
        var order = await orderService.GetOrder(id);

        if (order == null)
        {
            return NotFound();
        }

        var administrator = new UserOrderResponse(order.Administrator.Id, order.Administrator.FullName);
        var employee = new UserOrderResponse(order.Employee.Id, order.Employee.FullName);
        var car = new CarResponse(order.CustomerCar.Car.Id, order.CustomerCar.Car.Model,
            order.CustomerCar.Car.Brand.Name);
        var customerCar = new CustomerCarResponse(order.CustomerCar.Id, car, order.CustomerCar.Customer.FullName,
            order.CustomerCar.Customer.Email, order.CustomerCar.Year, order.CustomerCar.Number);
        var services = order.Services.Select(x => new ServiceResponse(x.Id, x.Name, x.Price, x.Time)).ToList();
        return new OrderResponse(order.Id, order.Status, order.StartDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
            order.EndDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"), order.TotalPrice, order.TotalTime, administrator, employee,
            customerCar, services);
    }

    // PUT: api/Order/5
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(Role.Administrator))]
    public async Task<IActionResult> PutOrder(int id, OrderUpdateRequest request)
    {
        var result =
            await orderService.UpdateOrder(
                id, 
                request.AdministratorId,
                request.EmployeeId,
                request.CustomerCarId);
        
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok("Updated");
    }

    // POST: api/Order
    [HttpPost]
    [Authorize(Roles = nameof(Role.Administrator))]
    public async Task<ActionResult<OrderResponse>> PostOrder(OrderRequest request)
    {
        var (_, isFailure, order, error) = await orderService.AddOrder(
            request.AdministratorId,
            request.EmployeeId,
            request.CustomerCarId,
            request.ServiceIds
        );
        if (isFailure) return BadRequest(error);

        var administrator = new UserOrderResponse(order.Administrator.Id, order.Administrator.FullName);
        var employee = new UserOrderResponse(order.Employee.Id, order.Employee.FullName);
        var car = new CarResponse(order.CustomerCar.Car.Id, order.CustomerCar.Car.Model,
            order.CustomerCar.Car.Brand.Name);
        var customerCar = new CustomerCarResponse(order.CustomerCar.Id, car, order.CustomerCar.Customer.FullName,
            order.CustomerCar.Customer.Email, order.CustomerCar.Year, order.CustomerCar.Number);
        var services = order.Services.Select(x => new ServiceResponse(x.Id, x.Name, x.Price, x.Time)).ToList();
        var response = new OrderResponse(order.Id, order.Status, order.StartDate.ToLocalTime().ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
            order.EndDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"), order.TotalPrice, order.TotalTime, administrator, employee,
            customerCar,
            services);

        return CreatedAtAction("GetOrders", new { id = order.Id }, response);
    }

    // DELETE: api/Order/5
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(Role.Administrator))]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var service = await orderService.GetOrder(id);
        if (service == null)
        {
            return NotFound();
        }

        await orderService.DeleteOrder(id);

        return Ok("Deleted");
    }
    
    // POST: api/Order/5/AddServices
    [HttpPost("{id:int}/AddServices")]
    [Authorize(Roles = nameof(Role.Administrator))]
    public async Task<IActionResult> AddServices(int id, [FromBody] List<int> serviceIds)
    {
        var service = await orderService.GetOrder(id);
        if (service == null) return NotFound("Order not found");

        var result = await orderService.AddServices(id, serviceIds);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok("Services added");
    }
    
    // POST: api/Order/5/Complete
    [HttpPost("{id:int}/Complete")]
    [Authorize(Roles = nameof(Role.Administrator))]
    public async Task<IActionResult> CompleteOrder(int id)
    {
        var service = await orderService.GetOrder(id);
        if (service == null) return NotFound("Order not found");

        var result = await orderService.CompleteOrder(id);

        if (result.IsFailure) return BadRequest(result.Error);

        return Ok("Order completed");
    }
}