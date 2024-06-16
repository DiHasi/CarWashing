using CarWashing.Application.Services;
using CarWashing.Contracts.Brand;
using CarWashing.Domain.Enums;
using CarWashing.Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = nameof(Role.Administrator))]
public class BrandController(BrandService brandService) : ControllerBase
{
    // GET: api/Brand
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BrandResponse>>> GetBrands([FromQuery] BrandFilter filter)
    {
        var brands = await brandService.GetBrands(filter);
        return Ok(brands);
    }
    // GET: api/Brand/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BrandResponse>> GetBrand(int id)
    {
        var brand = await brandService.GetBrand(id);
        
        if (brand == null)
        {
            return NotFound();
        }
        
        return new BrandResponse(brand.Id, brand.Name);
    }
    
    // PUT: api/Brand/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutBrand(int id, [FromBody]string name)
    {
        var result = await brandService.UpdateBrand(id, name);
        if(result.IsFailure) return BadRequest(result.Error);
        return Ok("Updated");
    }

    // POST: api/Brand
    [HttpPost]
    public async Task<ActionResult<BrandResponse>> PostBrand([FromBody]string name)
    {
        var result = await brandService.AddBrand(name);
        if(result.IsFailure) return BadRequest(result.Error);
        
        var response = new BrandResponse(result.Value.Id, result.Value.Name);
        
        return CreatedAtAction("GetBrands", new { id = result.Value.Id }, response);
    }

    // DELETE: api/Brand/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var service = await brandService.GetBrand(id);
        if (service == null)
        {
            return NotFound();
        }

        await brandService.DeleteBrand(id);

        return Ok("Deleted");
    }
}