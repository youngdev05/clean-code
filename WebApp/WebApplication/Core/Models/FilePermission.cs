namespace WebApplication.Core.Models;

public class FilePermission
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public int UserId { get; set; }
    public string PermissionType { get; set; } // 'Editor' или 'Viewer'

    public File File { get; set; }
    public User User { get; set; }
}