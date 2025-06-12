using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PopsicleFactory.Api.Models;
using PopsicleFactory.Api.Repositories;

namespace PopsicleFactory.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController(IInventoryRepository inventoryRepository, IValidator<PopsicleModel> validator) 
    : ControllerBase
{
    [HttpGet("{id:int}", Name = "GetPopsicles")]
    public async Task<IActionResult> GetPopsicle(int id)
    {
        var popsicle = await inventoryRepository.GetPopsicleByIdAsync(id);
        if (popsicle is null)
        {
            return NotFound();
        }

        return Ok(popsicle);
    }

    [HttpPost(Name = "CreatePopsicle")]
    public async Task<IActionResult> CreatePopsicle([FromBody] PopsicleModel popsicle)
    {
        var validationResult = await validator.ValidateAsync(popsicle);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdPopsicle = await inventoryRepository.CreatePopsicleAsync(popsicle);
        
        return CreatedAtAction(nameof(GetPopsicle), new { id = createdPopsicle.Id }, createdPopsicle);
    }

    [HttpPut("{id:int}", Name = "UpdatePopsicle")]
    public async Task<IActionResult> UpdatePopsicle(int id, [FromBody] PopsicleModel popsicle)
    {
        if (id != popsicle.Id)
        {
            return BadRequest("ID in URL does not match ID in request body");
        }

        // Validate the request
        var validationResult = await validator.ValidateAsync(popsicle);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var existingPopsicle = await inventoryRepository.GetPopsicleByIdAsync(id);
        if (existingPopsicle is null)
        {
            return NotFound();
        }

        var updatedPopsicle = await inventoryRepository.UpdatePopsicleAsync(popsicle);
        
        return Ok(updatedPopsicle);
    }

    [HttpDelete("{id:int}", Name = "DeletePopsicle")]
    public async Task<IActionResult> DeletePopsicle(int id)
    {
        var deleted = await inventoryRepository.DeletePopsicleAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet(Name = "GetAllPopsicles")]
    public async Task<IActionResult> GetAllPopsicles()
    {
        var popsicles = await inventoryRepository.GetAllPopsicles();
        return Ok(popsicles);
    }

    [HttpGet("search", Name = "SearchPopsicles")]
    public async Task<IActionResult> SearchPopsicles([FromQuery] string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest("Search term is required");
        }

        var popsicles = await inventoryRepository.SearchPopsiclesAsync(searchTerm);
        return Ok(popsicles);
    }
}