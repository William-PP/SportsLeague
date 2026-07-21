namespace SportsLeague.API.DTOs.Request;

public class AuthRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}