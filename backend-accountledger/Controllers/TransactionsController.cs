using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BackendAccountLedger.Application.Interfaces;
using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = "Basic")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionsController(ITransactionService service) => _service = service;

    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetByAccountId(int accountId)
    {
        var transactions = await _service.GetByAccountIdAsync(accountId);
        return Ok(transactions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var transaction = await _service.GetByIdAsync(id);
        return transaction == null 
            ? NotFound(new { title = "Not Found", detail = "Transaction not found." }) 
            : Ok(transaction);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateTransactionDto dto)
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

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] UpdateTransactionDto dto)
    {
        try
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { title = "Validation Failed", detail = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { title = "Not Found", detail = "Transaction not found." });
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
            return NotFound(new { title = "Not Found", detail = "Transaction not found." });
        }
    }
}