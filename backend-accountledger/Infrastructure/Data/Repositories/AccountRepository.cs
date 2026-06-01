using BackendAccountLedger.Application.DTOs;
using BackendAccountLedger.Application.Interfaces;
using Dapper;
using Npgsql;

namespace BackendAccountLedger.Infrastructure.Data.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly NpgsqlDataSource _db;

    public AccountRepository(NpgsqlDataSource db) => _db = db;

    public async Task<IEnumerable<AccountDto>> GetByPersonIdAsync(int personId)
    {
        using var conn = await _db.OpenConnectionAsync();
        return await conn.QueryAsync<AccountDto>("SELECT * FROM fn_get_accounts_by_person(@personId)", new { personId });
    }

    public async Task<AccountDto?> GetByIdAsync(int id)
    {
        using var conn = await _db.OpenConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<AccountDto>("SELECT * FROM fn_get_account_by_id(@id)", new { id });
    }

    public async Task<int> CreateAsync(CreateAccountDto dto)
    {
        using var conn = await _db.OpenConnectionAsync();
        return await conn.ExecuteScalarAsync<int>("SELECT fn_create_account(@person_id, @account_number)", dto);
    }

    public async Task UpdateAsync(int id, UpdateAccountDto dto)
    {
        using var conn = await _db.OpenConnectionAsync();
        await conn.ExecuteAsync("SELECT fn_update_account(@id, @account_number)", new { id, dto.account_number });
    }

    public async Task ToggleStatusAsync(int id, bool isOpen)
    {
        using var conn = await _db.OpenConnectionAsync();
        // Parameter names match DB function exactly
        await conn.ExecuteAsync("SELECT fn_toggle_account_status(@p_account_id, @p_is_open)",
            new { p_account_id = id, p_is_open = isOpen });
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(int accountId)
    {
        using var conn = await _db.OpenConnectionAsync();
        return await conn.QueryAsync<TransactionDto>("SELECT * FROM fn_get_transactions_by_account(@accountId)", new { accountId });
    }

    public async Task<bool> AccountNumberExistsAsync(string accountNumber)
    {
        using var conn = await _db.OpenConnectionAsync();
        return await conn.ExecuteScalarAsync<bool>("SELECT EXISTS(SELECT 1 FROM accounts WHERE account_number = @accountNumber)", new { accountNumber });
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = await _db.OpenConnectionAsync();
        await conn.ExecuteAsync("DELETE FROM accounts WHERE id = @id", new { id });
    }
}