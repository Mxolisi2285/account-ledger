using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendAccountLedger.Application.Interfaces;
using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Basic")]
public class PersonsController : ControllerBase
{
    private readonly IPersonService _service;

    public PersonsController(IPersonService service) => _service = service;

    
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? searchType = null, 
        [FromQuery] string? searchTerm = null)
    {
        pageSize = Math.Clamp(pageSize, 1, 10);
        var result = await _service.GetPagedAsync(page, pageSize, searchType, searchTerm);
        
        
        return Ok(new { 
            items = result.Items, 
            totalCount = result.TotalCount,
            currentPage = result.CurrentPage,
            pageSize = result.PageSize
        });
    }

    /// <summary>
    /// Get single person by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var person = await _service.GetByIdAsync(id);
        if (person == null)
        {
            return NotFound(new { title = "Not Found", detail = "Person not found." });
        }
        return Ok(person); // PersonDto already uses snake_case properties
    }

    
    [HttpGet("check-id/{idNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckId(string idNumber)
    {
        var exists = await _service.IdNumberExistsAsync(idNumber);
        return Ok(new { exists });
    }

    /// <summary>
    /// Create new person (Rule 1: Unique ID enforced)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreatePersonDto dto)
    {
        try
        {
            var newId = await _service.CreateAsync(dto);
            
            return CreatedAtAction(nameof(Get), new { id = newId }, dto);
        }
        catch (ArgumentException ex)
        {
            
            return BadRequest(new { title = "Validation Failed", detail = ex.Message });
        }
    }

    /// <summary>
    /// Update existing person
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdatePersonDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent(); // 204 No Content on success
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { title = "Validation Failed", detail = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { title = "Not Found", detail = "Person not found." });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            
            return BadRequest(new { title = "Delete Failed", detail = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { title = "Not Found", detail = "Person not found." });
        }
    }
}