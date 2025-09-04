using Microsoft.AspNetCore.Mvc;
using PU.Users.Api.Services;
using PU.Users.Api.DTOs;

namespace PU.Users.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly GroupService _svc;

    public GroupsController(GroupService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GroupDto>>> GetAll()
    {
        var groups = await _svc.GetAllAsync();
        var dtos = groups.Select(g => new GroupDto(g.Id, g.Name, g.GroupPermissions.Select(gp => gp.PermissionId).ToArray()));
        return Ok(dtos);
    }

    public record CreateGroup(string Name, int[] PermissionIds);

    [HttpPost]
    public async Task<ActionResult<GroupDto>> Create(CreateGroup input)
    {
        var g = await _svc.CreateAsync(input.Name, input.PermissionIds);
        return CreatedAtAction(nameof(GetAll), null, new GroupDto(g.Id, g.Name, input.PermissionIds));
    }

    [HttpPost("{groupId:int}/users/{userId:int}")]
    public async Task<IActionResult> AddUser(int groupId, int userId)
    {
        await _svc.AddUserAsync(groupId, userId);
        return NoContent();
    }

    [HttpDelete("{groupId:int}/users/{userId:int}")]
    public async Task<IActionResult> RemoveUser(int groupId, int userId)
    {
        await _svc.RemoveUserAsync(groupId, userId);
        return NoContent();
    }

    [HttpGet("userCounts")]
    public async Task<ActionResult<Dictionary<int, int>>> UsersPerGroup()
        => Ok(await _svc.UsersPerGroupAsync());
}
