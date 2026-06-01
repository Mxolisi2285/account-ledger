using BackendAccountLedger.Application.DTOs;
using BackendAccountLedger.Application.Interfaces;
using Npgsql;

namespace BackendAccountLedger.Application.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _repository;
    public PersonService(IPersonRepository repository) => _repository = repository;

    public async Task<PagedResult<PersonDto>> GetPagedAsync(int page, int pageSize, string? searchType, string? searchTerm)
        => await _repository.GetPagedAsync(page, pageSize, searchType, searchTerm);

    public async Task<PersonDto?> GetByIdAsync(int id) => await _repository.GetByIdAsync(id);

    public async Task<int> CreateAsync(CreatePersonDto dto)
    {
        try
        {
            return await _repository.CreateAsync(dto);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505") // Unique violation
        {
            
            throw new ArgumentException("A person with this ID Number already exists.", nameof(dto.id_number));
        }
    }

    public async Task UpdateAsync(int id, UpdatePersonDto dto)
    {
        try
        {
            await _repository.UpdateAsync(id, dto);
        }
        catch (Npgsql.PostgresException ex) when (ex.SqlState == "23505")
        {
            
            throw new ArgumentException("Update failed due to a constraint violation.", "dto");
        }
    }

    public async Task DeleteAsync(int id) => await _repository.DeleteAsync(id);
    
    public async Task<bool> IdNumberExistsAsync(string idNumber) => await _repository.IdNumberExistsAsync(idNumber);
}