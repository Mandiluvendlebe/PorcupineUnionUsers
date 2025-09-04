namespace PU.Users.Api.Models;

public class UserGroup
{
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public int GroupId { get; set; }
    public Group Group { get; set; } = default!;
}
