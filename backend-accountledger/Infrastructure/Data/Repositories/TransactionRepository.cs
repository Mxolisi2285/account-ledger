using BackendAccountLedger.Application.DTOs;
using BackendAccountLedger.Application.Interfaces;
using Dapper;
using Npgsql;

namespace BackendAccountLedger.Infrastructure.Data.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly NpgsqlDataSource _db;

    public TransactionRepository(NpgsqlDataSource db) => _db = db;

    public async Task<IEnumerable<TransactionDto>> GetByAccountIdAsync(int accountId)
    {
        using var conn = await _db.OpenConnectionAsync();
        return await conn.QueryAsync<TransactionDto>("SELECT * FROM fn_get_transactions_by_account(@accountId)", new { accountId });
    }

    public async Task<TransactionDto?> GetByIdAsync(int id)
    {
        using var conn = await _db.OpenConnectionAsync();
        return await conn.QueryFirstOrDefaultAsync<TransactionDto>("SELECT * FROM fn_get_transaction_by_id(@id)", new { id });
    }

    public async Task<int> CreateAsync(CreateTransactionDto dto)
    {
        using var conn = await _db.OpenConnectionAsync();
        //  Parameter names match DB function + explicit ::date cast to avoid timestamp mismatch
        return await conn.ExecuteScalarAsync<int>(
            "SELECT fn_create_transaction(@p_account_id, @p_transaction_date::date, @p_amount, @p_type)",
            new
            {
                p_account_id = dto.account_id,
                p_transaction_date = dto.transaction_date,
                p_amount = dto.amount,
                p_type = dto.type
            });
    }

    public async Task UpdateAsync(int id, UpdateTransactionDto dto)
    {
        using var conn = await _db.OpenConnectionAsync();
        // Parameter names match DB function
        await conn.ExecuteAsync(
            "SELECT fn_update_transaction(@p_id, @p_transaction_date::date, @p_amount, @p_type)",
            new
            {
                p_id = id,
                p_transaction_date = dto.transaction_date,
                p_amount = dto.amount,
                p_type = dto.type
            });
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = await _db.OpenConnectionAsync();
        await conn.ExecuteAsync("DELETE FROM transactions WHERE id = @id", new { id });
    }
}