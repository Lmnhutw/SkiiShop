namespace SShopAPI.DTOs;

public sealed record ProductListResponse(
    IReadOnlyList<Core.Entities.Product> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);
