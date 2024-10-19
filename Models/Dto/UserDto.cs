namespace MemoryBox_API.Models.Dto;

public class UserRegisterDto
{
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    public string? Email { get; set; }
    public string? avatarURL { get; set; }
}

public class UserLoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}