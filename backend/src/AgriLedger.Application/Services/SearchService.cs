using AgriLedger.Application.DTOs.Search;
using AgriLedger.Application.Interfaces;

namespace AgriLedger.Application.Services;

// Powers the spec's "Global Search" - searches farm, crop, buyer, worker,
// category, and expense description in one call.
public class SearchService : ISearchService
{
    private readonly IUnitOfWork _uow;

    public SearchService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<GlobalSearchResultDto> SearchAsync(int userId, string term)
    {
        var t = term.Trim().ToLower();
        if (string.IsNullOrWhiteSpace(t)) return new GlobalSearchResultDto();

        var farms = _uow.Farms.Query()
            .Where(f => f.UserId == userId && !f.IsDeleted && f.Name.ToLower().Contains(t))
            .Select(f => new SearchResultItemDto { Id = f.Id, Title = f.Name, Subtitle = f.Location ?? "" })
            .Take(10).ToList();

        var crops = _uow.Crops.Query()
            .Where(c => !c.IsDeleted && c.Farm!.UserId == userId && c.Name.ToLower().Contains(t))
            .Select(c => new SearchResultItemDto { Id = c.Id, Title = c.Name, Subtitle = c.Farm!.Name })
            .Take(10).ToList();

        var expenses = _uow.Expenses.Query()
            .Where(e => e.UserId == userId && !e.IsDeleted &&
                ((e.Description != null && e.Description.ToLower().Contains(t)) ||
                 e.ExpenseCategory!.Name.ToLower().Contains(t)))
            .Select(e => new SearchResultItemDto { Id = e.Id, Title = e.ExpenseCategory!.Name, Subtitle = e.Description ?? "" })
            .Take(10).ToList();

        var incomes = _uow.Incomes.Query()
            .Where(i => i.UserId == userId && !i.IsDeleted && i.BuyerName.ToLower().Contains(t))
            .Select(i => new SearchResultItemDto { Id = i.Id, Title = i.BuyerName, Subtitle = i.Farm!.Name })
            .Take(10).ToList();

        var labors = _uow.Labors.Query()
            .Where(l => l.UserId == userId && !l.IsDeleted && l.WorkerName.ToLower().Contains(t))
            .Select(l => new SearchResultItemDto { Id = l.Id, Title = l.WorkerName, Subtitle = l.Task })
            .Take(10).ToList();

        return new GlobalSearchResultDto { Farms = farms, Crops = crops, Expenses = expenses, Incomes = incomes, Labors = labors };
    }
}
