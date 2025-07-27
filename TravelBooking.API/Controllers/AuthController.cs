using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TravelBooking.Domain.Entities;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static List<User> Users = new(); // TEMP in-memory DB

    private readonly IConfiguration _config;

    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        var exists = await _context.Users.AnyAsync(u => u.Email == user.Email);
        if (exists)
            return BadRequest("Email already exists.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
        user.Role = "User";

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Ok("User registered in the database.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User login)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        var token = CreateToken(user);
        return Ok(new { token });
    }

    private string CreateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Authorize]
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API is up and running");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin-only")]
    public IActionResult AdminZone()
    {
        return Ok("You've been arrived at admin test zone");
    }

}
