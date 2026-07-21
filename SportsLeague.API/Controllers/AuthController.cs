using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportsLeague.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDTO>> Register(RegisterRequestDTO request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = UserRole.Viewer
        };

        try
        {
            var created = await _userService.CreateAsync(user, request.Password);
            var token = GenerateToken(created);

            return Ok(new AuthResponseDTO
            {
                Token = token,
                Email = created.Email,
                Role = created.Role.ToString(),
                Expiration = DateTime.UtcNow.AddMinutes(60)
            });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDTO>> Login(AuthRequestDTO request)
    {
        var isValid = await _userService.ValidatePasswordAsync(request.Email, request.Password);
        if (!isValid)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var user = await _userService.GetByEmailAsync(request.Email);
        var token = GenerateToken(user!);

        return Ok(new AuthResponseDTO
        {
            Token = token,
            Email = user!.Email,
            Role = user.Role.ToString(),
            Expiration = DateTime.UtcNow.AddMinutes(60)
        });
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}