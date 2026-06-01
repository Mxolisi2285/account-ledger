using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Application.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetByPersonIdAsync(int personId);
    Task<AccountDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateAccountDto dto);
    Task UpdateAsync(int id, UpdateAccountDto dto);
    Task ToggleStatusAsync(int id, bool isOpen);
    Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(int accountId);
    Task DeleteAsync(int id);
}