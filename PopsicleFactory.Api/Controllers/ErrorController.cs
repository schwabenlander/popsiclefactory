using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace PopsicleFactory.Api.Controllers;

[ApiController]
[Route("/")]
public class ErrorController(ILogger<ErrorController> logger) : ControllerBase
{
    [HttpGet("error")]
    public IActionResult Error()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        
        if (exception is null)
        {
            return Problem("An unknown error occurred.");
        }
        
        // Log the exception
        logger.LogError(exception, "An error occurred while processing the request.");
        
        return Problem();
    }
}