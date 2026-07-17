using AgriLedger.Application.DTOs.Common;
using AgriLedger.Application.DTOs.Search;
using AgriLedger.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[Authorize]
public class SearchController : BaseApiController
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
    {
        var results = await _searchService.SearchAsync(CurrentUserId, q ?? string.Empty);
        return Ok(ApiResponse<GlobalSearchResultDto>.Ok(results));
    }
}
