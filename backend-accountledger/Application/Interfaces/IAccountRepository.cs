using BackendAccountLedger.Application.DTOs;

namespace BackendAccountLedger.Application.Interfaces; // ✅ Fixed namespace

public interface IAccountRepository
{
    Task<IEnumerable<AccountDto>> GetByPersonIdAsync(int personId);
    Task<AccountDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateAccountDto dto);
    Task UpdateAsync(int id, UpdateAccountDto dto);
    Task ToggleStatusAsync(int id, bool isOpen);
    Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(int accountId);
    Task<bool> AccountNumberExistsAsync(string accountNumber); // ✅ Don't forget this
    Task DeleteAsync(int id);
}