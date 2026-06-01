using BackendAccountLedger.Application.DTOs;
using BackendAccountLedger.Application.Interfaces;

namespace BackendAccountLedger.Application.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _repository;
    public AccountService(IAccountRepository repository) => _repository = repository;

    public async Task<IEnumerable<AccountDto>> GetByPersonIdAsync(int personId)
        => await _repository.GetByPersonIdAsync(personId);

    public async Task<AccountDto?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    // Single CreateAsync with uniqueness check
    public async Task<int> CreateAsync(CreateAccountDto dto)
    {
        if (dto.person_id <= 0)
            throw new ArgumentException("Valid Person ID required.", nameof(dto.person_id));

        var exists = await _repository.AccountNumberExistsAsync(dto.account_number);
        if (exists)
            throw new ArgumentException("An account with this number already exists.");

        return await _repository.CreateAsync(dto);
    }

    public async Task UpdateAsync(int id, UpdateAccountDto dto)
        => await _repository.UpdateAsync(id, dto);

    public async Task ToggleStatusAsync(int id, bool isOpen)
        => await _repository.ToggleStatusAsync(id, isOpen);

    public async Task<IEnumerable<TransactionDto>> GetTransactionsByAccountIdAsync(int accountId)
        => await _repository.GetTransactionsByAccountIdAsync(accountId);

    public async Task DeleteAsync(int id)
        => await _repository.DeleteAsync(id);
}