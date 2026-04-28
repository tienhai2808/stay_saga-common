namespace Common.DTOs;

public sealed record MetaResponseDto(
    int Total,
    int Page,
    int Limit,
    int TotalPage,
    bool HasPrev,
    bool HasNext
);