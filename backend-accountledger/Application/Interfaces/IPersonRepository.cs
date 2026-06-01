using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Application.Interfaces;

/// <summary>
/// Data access contract for Persons. Uses Stored Procedures via Dapper.
/// </summary>
public interface IPersonRepository
{
    /// <summary>
    /// Rule 2: Search by ID Number, Surname, or Account Number. Rule: Max 10/page.
    /// </summary>
    Task<PagedResult<PersonDto>> GetPagedAsync(int page, int pageSize, string? searchType, string? searchTerm);

    Task<PersonDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreatePersonDto dto);
    Task UpdateAsync(int id, UpdatePersonDto dto);

    /// <summary>
    /// Rule 3: Only deletes if person has no accounts or all accounts are closed.
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// Rule 1: Validates unique ID Number constraint.
    /// </summary>
    Task<bool> IdNumberExistsAsync(string idNumber);
}