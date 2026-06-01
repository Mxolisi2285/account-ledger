namespace BackendAccountLedger.Application.DTOs;

public record TransactionDto(
    int id,
    int account_id,
    DateTime transaction_date,
    decimal amount,
    string type,
    DateTime capture_date,
    DateTime modified_date
);

public record CreateTransactionDto(
    int account_id,
    DateTime transaction_date,
    decimal amount,
    string type
);

public record UpdateTransactionDto(
    DateTime transaction_date,
    decimal amount,
    string type
);