using BackendAccountLedger.Application.DTOs;
using BackendAccountLedger.Application.Interfaces;

namespace BackendAccountLedger.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repository;

    public TransactionService(ITransactionRepository repository) => _repository = repository;

    public async Task<IEnumerable<TransactionDto>> GetByAccountIdAsync(int accountId)
        => await _repository.GetByAccountIdAsync(accountId);

    public async Task<TransactionDto?> GetByIdAsync(int id)
        => await _repository.GetByIdAsync(id);

    public async Task<int> CreateAsync(CreateTransactionDto dto)
        => await _repository.CreateAsync(dto);

    public async Task UpdateAsync(int id, UpdateTransactionDto dto)
        => await _repository.UpdateAsync(id, dto);

    
    public async Task DeleteAsync(int id)
        => await _repository.DeleteAsync(id);
}