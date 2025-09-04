namespace PU.Users.Api.Models;

public class GroupPermission
{
    public int GroupId { get; set; }
    public Group Group { get; set; } = default!;

    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = default!;
}
