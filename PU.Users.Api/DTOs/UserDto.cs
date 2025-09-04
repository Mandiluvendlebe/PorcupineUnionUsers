namespace PU.Users.Api.DTOs;

public record UserDto(int Id, string FirstName, string LastName, string Email, int[] GroupIds);
