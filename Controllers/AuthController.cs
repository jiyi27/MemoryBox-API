using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MemoryBox_API.Models;
using MemoryBox_API.Models.Dto;
using MemoryBox_API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MemoryBox_API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(DatabaseContext context, IConfiguration configuration, ILogger<AuthController> logger) : ControllerBase
{
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
    {
        var existingUser = await context.Users
            .FirstOrDefaultAsync(u => u.Username == userRegisterDto.Username);
        if (existingUser != null)
        {
            return Conflict(new { message = "Username already exist." });
        }

        var user = new User
        {
            FullName = userRegisterDto.FullName,
            Username = userRegisterDto.Username,
            Password = userRegisterDto.Password,
            Email = userRegisterDto.Email,
            ProfilePictureUrl = userRegisterDto.ProfilePictureUrl
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Created();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto userLoginDto)
    {
        var user = await AuthenticateUser(userLoginDto);
        if (user == null)
        {
            return BadRequest(new { message = "Invalid username or password." });
        }

        var token = GenerateToken(user);
        return Ok(new {token});
    }
    
    private async Task<User?> AuthenticateUser(UserLoginDto userLoginDto)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == userLoginDto.Username);
        // print user, don't use Console.WriteLine which will not be printed in console.
        // logger.LogInformation("User: {Username}, {Password}", user?.Username, user?.Password);
        
        if (user == null) return null;
        return user.Password == userLoginDto.Password ? user : null;
    }
    
    private string GenerateToken(User user)
    {
        var jwtKey = configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("Jwt:Key is not configured");
        }
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
        };
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}