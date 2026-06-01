namespace BackendAccountLedger.Application.DTOs;

public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int CurrentPage,
    int PageSize
);