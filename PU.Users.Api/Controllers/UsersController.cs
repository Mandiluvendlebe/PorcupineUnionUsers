using Microsoft.AspNetCore.Mvc;
using PU.Users.Api.Services;
using PU.Users.Api.DTOs;
using PU.Users.Api.Models;

namespace PU.Users.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _svc;

    public UsersController(UserService svc) => _svc = svc;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        var users = await _svc.GetAllAsync(page, pageSize);
        var dtos = users.Select(u => new UserDto(u.Id, u.FirstName, u.LastName, u.Email, u.UserGroups.Select(ug => ug.GroupId).ToArray()));
        return Ok(dtos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> Get(int id)
    {
        var u = await _svc.GetAsync(id);
        if (u is null) return NotFound();
        return Ok(new UserDto(u.Id, u.FirstName, u.LastName, u.Email, u.UserGroups.Select(ug => ug.GroupId).ToArray()));
    }

        public class UpsertUser
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string FirstName { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        public string LastName { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.EmailAddress]
        public string Email { get; set; } = string.Empty;
        public int[] GroupIds { get; set; } = Array.Empty<int>();
    }
[HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] UpsertUser input)
    {
        var user = await _svc.CreateAsync(new User {
            FirstName = input.FirstName, LastName = input.LastName, Email = input.Email
        }, input.GroupIds);
        return CreatedAtAction(nameof(Get), new { id = user.Id },
            new UserDto(user.Id, user.FirstName, user.LastName, user.Email, input.GroupIds));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertUser input)
    {
        var ok = await _svc.UpdateAsync(id, new User {
            FirstName = input.FirstName, LastName = input.LastName, Email = input.Email
        }, input.GroupIds);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _svc.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("count")]
    public async Task<ActionResult<int>> Count() => Ok(await _svc.CountAsync());
}
