namespace SurveyBasket.Core.Contracts.Commen;

public record RequestFilter
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchValue { get; init; }
    public string? SortColumn { get; init; }
    public string? SortDirection { get; init; } = "ASC";
}
