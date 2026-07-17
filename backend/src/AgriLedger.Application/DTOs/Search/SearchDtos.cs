namespace AgriLedger.Application.DTOs.Search;

public class GlobalSearchResultDto
{
    public List<SearchResultItemDto> Farms { get; set; } = new();
    public List<SearchResultItemDto> Crops { get; set; } = new();
    public List<SearchResultItemDto> Expenses { get; set; } = new();
    public List<SearchResultItemDto> Incomes { get; set; } = new();
    public List<SearchResultItemDto> Labors { get; set; } = new();
}

public class SearchResultItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
}
