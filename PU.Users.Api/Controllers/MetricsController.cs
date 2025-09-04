using Microsoft.AspNetCore.Mvc;
using PU.Users.Api.Services;

namespace PU.Users.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly UserService _userSvc;
    private readonly GroupService _groupSvc;

    public MetricsController(UserService userSvc, GroupService groupSvc)
    {
        _userSvc = userSvc;
        _groupSvc = groupSvc;
    }

    [HttpGet("userCount")]
    public async Task<ActionResult<int>> UserCount() => Ok(await _userSvc.CountAsync());

    [HttpGet("usersPerGroup")]
    public async Task<ActionResult<Dictionary<int, int>>> UsersPerGroup() => Ok(await _groupSvc.UsersPerGroupAsync());
}
