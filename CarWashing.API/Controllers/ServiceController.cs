using CarWashing.Application.Services;
using CarWashing.Contracts.Service;
using CarWashing.Domain.Filters;
using Microsoft.AspNetCore.Mvc;
using CarWashing.Domain.Models;
using CarWashing.Domain.ValueObjects;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ServiceController(ServiceService serviceService) : ControllerBase
{
    // GET: api/Service
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetServices([FromQuery] ServiceFilter filter)
    {
        var services = await serviceService.GetServices(filter);
        var serviceResponses = services
            .Select(service => new ServiceResponse(service.Id, service.Name, service.Price, service.Time));
        return Ok(serviceResponses);
    }
    
    // GET: api/Service/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceResponse>> GetService(int id)
    {
        var service = await serviceService.GetService(id);
        
        if (service == null)
        {
            return NotFound();
        }
        
        return new ServiceResponse(service.Id, service.Name, service.Price, service.Time);
    }

    // PUT: api/Service/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutService(int id, ServiceRequest service)
    {
        var serviceToUpdate = await serviceService.GetService(id);
        if(serviceToUpdate == null) return NotFound();
        
        var result = serviceToUpdate
            .ChangeName(service.Name).Value
            .ChangePrice(Price.Create(service.Price)).Value
            .ChangeTime(Time.Create(service.Time));
            
        if(result.IsFailure) return BadRequest(result.Error);
            
            
        await serviceService.UpdateService(id, result.Value
        );
            
        return Ok("Updated");
    }

    // POST: api/Service
    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> PostService(ServiceRequest serviceRequest)
    {
        var result = Service.Create(serviceRequest.Name,
            Price.Create(serviceRequest.Price),
            Time.Create(serviceRequest.Time));
            
        if(result.IsFailure) return BadRequest(result.Error);
            
        var service = await serviceService.AddService(result.Value);

        var response = new ServiceResponse(service.Id, service.Name, service.Price, service.Time);
        return CreatedAtAction("GetService", new { id = service.Id }, response);
    }

    // DELETE: api/Service/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var service = await serviceService.GetService(id);
        if (service == null)
        {
            return NotFound();
        }

        await serviceService.DeleteService(id);

        return Ok("Deleted");
    }
}