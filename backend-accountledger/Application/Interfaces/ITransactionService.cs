using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Application.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionDto>> GetByAccountIdAsync(int accountId);
    Task<TransactionDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateTransactionDto dto);
    Task UpdateAsync(int id, UpdateTransactionDto dto);
    Task DeleteAsync(int id);
}