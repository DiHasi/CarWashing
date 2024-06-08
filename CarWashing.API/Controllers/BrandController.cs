using CarWashing.Application.Services;
using CarWashing.Contracts.Brand;
using CarWashing.Domain.Filters;
using CarWashing.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CarWashing.Controllers;

[Route("api/[controller]")]
[ApiController]
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
        var brandToUpdate = await brandService.GetBrand(id);
        
        if(brandToUpdate == null) return NotFound();
        
        // var result = Brand.Create(name);
        //     
        // if(result.IsFailure) return BadRequest(result.Error);
            
        var result = brandToUpdate.ChangeName(name);
        
        if(result.IsFailure) return BadRequest(result.Error);
        
        await brandService.UpdateBrand(id, result.Value);
            
        return Ok("Updated");
    }

    // POST: api/Brand
    [HttpPost]
    public async Task<ActionResult<BrandResponse>> PostBrand([FromBody]string name)
    {
        var result = Brand.Create(name);
            
        if(result.IsFailure) return BadRequest(result.Error);
            
        var brand = await brandService.AddBrand(result.Value);

        var response = new BrandResponse(brand.Id, brand.Name);
        return CreatedAtAction("GetBrands", new { id = brand.Id }, response);
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