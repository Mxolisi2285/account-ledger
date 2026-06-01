using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendAccountLedger.Application.Interfaces;
using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Basic")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;
    public AccountsController(IAccountService service) => _service = service;

    [HttpGet("person/{personId}")]
    public async Task<IActionResult> GetByPersonId(int personId)
    {
        var accounts = await _service.GetByPersonIdAsync(personId);
        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var account = await _service.GetByIdAsync(id);
        return account == null ? NotFound(new { title = "Not Found", detail = "Account not found." }) : Ok(account);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateAccountDto dto)
    {
        try { var newId = await _service.CreateAsync(dto); return CreatedAtAction(nameof(Get), new { id = newId }, dto); }
        catch (ArgumentException ex) { return BadRequest(new { title = "Validation Failed", detail = ex.Message }); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateAccountDto dto)
    {
        try { await _service.UpdateAsync(id, dto); return NoContent(); }
        catch (ArgumentException ex) { return BadRequest(new { title = "Validation Failed", detail = ex.Message }); }
        catch (KeyNotFoundException) { return NotFound(new { title = "Not Found", detail = "Account not found." }); }
    }

    [HttpPost("{id}/toggle-status")]
    public async Task<IActionResult> ToggleStatus(int id, [FromQuery] bool isOpen)
    {
        try { await _service.ToggleStatusAsync(id, isOpen); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { title = "Status Change Failed", detail = ex.Message }); }
        catch (KeyNotFoundException) { return NotFound(new { title = "Not Found", detail = "Account not found." }); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _service.DeleteAsync(id); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { title = "Delete Failed", detail = ex.Message }); }
        catch (KeyNotFoundException) { return NotFound(new { title = "Not Found", detail = "Account not found." }); }
    }
}