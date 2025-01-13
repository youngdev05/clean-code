namespace Core.Models
{
    public class File
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Владелец
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User Owner { get; set; }
        public List<FilePermission> Permissions { get; set; } = new();
    }
}