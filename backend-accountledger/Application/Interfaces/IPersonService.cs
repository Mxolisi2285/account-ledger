using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Application.Interfaces;


public interface IPersonService
{
    Task<PagedResult<PersonDto>> GetPagedAsync(int page, int pageSize, string? searchType, string? searchTerm);
    Task<PersonDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreatePersonDto dto);
    Task UpdateAsync(int id, UpdatePersonDto dto);
    Task DeleteAsync(int id);
    Task<bool> IdNumberExistsAsync(string idNumber);
}