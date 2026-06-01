namespace BackendAccountLedger.Application.DTOs;

public record AccountDto(
    int id,
    int person_id,
    string account_number,
    decimal outstanding_balance,
    int status_id,
    string status_name,
    bool is_closed,
    DateTime opened_date
);

public record CreateAccountDto(
    int person_id,
    string account_number
);

public record UpdateAccountDto(
    string account_number
);