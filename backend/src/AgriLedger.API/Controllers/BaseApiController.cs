using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace AgriLedger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    // Reads the authenticated user's id from the JWT "sub" claim.
    protected int CurrentUserId =>
        int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? "0");
}
