using Microsoft.AspNetCore.Mvc;
using PopsicleFactory.Api.Models;
using PopsicleFactory.Api.Repositories;

namespace PopsicleFactory.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController(IInventoryRepository inventoryRepository, ILogger<InventoryController> logger) 
    : ControllerBase
{
    [HttpGet(Name = "GetPopsicles")]
    public async Task<IActionResult> GetPopsicle([FromQuery] int id)
    {
        var popsicle = await inventoryRepository.GetPopsicleByIdAsync(id);
        
        if (popsicle == null)
        {
            return NotFound();
        }

        return Ok(popsicle);
    }
}