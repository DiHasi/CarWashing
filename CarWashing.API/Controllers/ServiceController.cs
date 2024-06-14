using CarWashing.Application.Services;
using CarWashing.Contracts.Service;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using Microsoft.AspNetCore.Mvc;
using CarWashing.Domain.Models;
using CarWashing.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Role.Administrator))]
public class ServiceController(ServiceService serviceService) : ControllerBase
{
    // GET: api/Service
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetServices([FromQuery] ServiceFilter filter)
    {
        var services = await serviceService.GetServices(filter);
        var serviceResponses = services.Value
            .Select(service => new ServiceResponse(service.Id, service.Name, service.Price, service.Time));
        return Ok(serviceResponses);
    }
    
    // GET: api/Service/5
    [AllowAnonymous]
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
        var result = await serviceService.UpdateService(id, service.Name, service.Price, service.Time);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok("Updated");
    }

    // POST: api/Service
    [HttpPost]
    public async Task<ActionResult<ServiceResponse>> PostService(ServiceRequest request)
    {
        var result = await serviceService.AddService(request.Name, request.Price, request.Time);
        if(result.IsFailure) return BadRequest(result.Error);

        var response = new ServiceResponse(result.Value.Id, result.Value.Name, result.Value.Price, result.Value.Time);
        return CreatedAtAction("GetService", new { id = result.Value.Id }, response);
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