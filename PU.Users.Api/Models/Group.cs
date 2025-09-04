namespace PU.Users.Api.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
    public ICollection<GroupPermission> GroupPermissions { get; set; } = new List<GroupPermission>();
}
