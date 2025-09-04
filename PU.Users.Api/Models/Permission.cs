namespace PU.Users.Api.Models;

public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
}
