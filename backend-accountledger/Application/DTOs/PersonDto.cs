namespace BackendAccountLedger.Application.DTOs;

public record PersonDto(
    int id,
    string id_number,
    string first_name,
    string surname,
    string contact_info,
    DateTime created_date
);

public record CreatePersonDto(
    string id_number,
    string first_name,
    string surname,
    string contact_info
);

public record UpdatePersonDto(
    string first_name,
    string surname,
    string contact_info
);