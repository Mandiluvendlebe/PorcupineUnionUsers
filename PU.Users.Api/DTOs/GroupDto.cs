namespace PU.Users.Api.DTOs;

public record GroupDto(int Id, string Name, int[] PermissionIds);
