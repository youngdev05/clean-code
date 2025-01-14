namespace WebApplication.Core.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } = "Viewer"; // Admin, Editor, Viewer
    public List<FilePermission> Permissions { get; set; } = new();
}