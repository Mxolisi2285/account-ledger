using Dapper;
using System.Data;
using BackendAccountLedger.Application.DTOs;
using BackendAccountLedger.Application.Interfaces;

namespace BackendAccountLedger.Infrastructure.Data.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly IDbConnection _db;

    public PersonRepository(IDbConnection db)
    {
        _db = db;
    }

    // =========================
    // PAGED LIST
    // =========================
    public async Task<PagedResult<PersonDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? searchType,
        string? searchTerm)
    {
        var param = new
        {
            Page = page,
            PageSize = pageSize,
            SearchType = searchType ?? "",
            SearchTerm = searchTerm ?? ""
        };

        var items = await _db.QueryAsync<PersonDto>(
            "SELECT * FROM fn_get_persons_paged(@Page, @PageSize, @SearchType, @SearchTerm)",
            param
        );

        var totalCount = await _db.ExecuteScalarAsync<int>(
            "SELECT fn_get_persons_total(@SearchType, @SearchTerm)",
            param
        );

        return new PagedResult<PersonDto>(items, totalCount, page, pageSize);
    }

    // =========================
    // GET BY ID (FIXED - IMPORTANT)
    // =========================
    public async Task<PersonDto?> GetByIdAsync(int id)
    {
        return await _db.QueryFirstOrDefaultAsync<PersonDto>(
            "SELECT * FROM fn_get_person_by_id(@p_id)",
            new { p_id = id }
        );
    }

    // =========================
    // CREATE
    // =========================
    public async Task<int> CreateAsync(CreatePersonDto dto)
    {
        return await _db.ExecuteScalarAsync<int>(
            "SELECT fn_create_person(@id_number, @first_name, @surname, @contact_info)",
            dto
        );
    }

    // =========================
    // UPDATE (FIXED PARAM BINDING)
    // =========================
    public async Task UpdateAsync(int id, UpdatePersonDto dto)
    {
        await _db.ExecuteAsync(
            "SELECT fn_update_person(@Id, @first_name, @surname, @contact_info)",
            new
            {
                Id = id,
                first_name = dto.first_name,
                surname = dto.surname,
                contact_info = dto.contact_info
            }
        );
    }

    // =========================
    // DELETE
    // =========================
    public async Task DeleteAsync(int id)
    {
        await _db.ExecuteAsync(
            "SELECT fn_delete_person(@PersonId)",
            new { PersonId = id }
        );
    }

    // =========================
    // CHECK ID NUMBER EXISTS
    // =========================
    public async Task<bool> IdNumberExistsAsync(string idNumber)
    {
        const string sql =
            "SELECT EXISTS(SELECT 1 FROM persons WHERE id_number = @IdNumber)";

        return await _db.ExecuteScalarAsync<bool>(sql, new { IdNumber = idNumber });
    }
}