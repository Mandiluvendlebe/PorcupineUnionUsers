using Microsoft.AspNetCore.Mvc;
using PU.Users.Api.Services;
using PU.Users.Api.DTOs;

namespace PU.Users.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly PermissionService _svc;
    public PermissionsController(PermissionService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PermissionDto>>> GetAll()
    {
        var list = await _svc.GetAllAsync();
        return Ok(list.Select(p => new PermissionDto(p.Id, p.Name)));
    }
}
