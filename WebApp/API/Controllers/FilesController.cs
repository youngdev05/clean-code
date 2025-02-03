using System.Security.Claims;
using API.DTOs;
using Markdown.Classes;
using MdProcessor.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [ApiController]
    [Route("api/files")]
    //[Authorize]
    public class FilesController : ControllerBase
    {
        private readonly AddDbContext _context;

        public FilesController(AddDbContext context)
        {
            _context = context;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { message = "Не авторизован" });

            var files = await _context.Files
                .Where(f => f.UserId == userId || f.Permissions.Any(p => p.UserId == userId))
                .Select(f => new { f.Id, f.Title, Owner = f.Owner.Username })
                .ToListAsync();

            return Ok(files);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { message = "Не авторизован" });

            var file = await _context.Files
                .Include(f => f.Permissions)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            if (file.UserId != userId && !file.Permissions.Any(p => p.UserId == userId))
                return Forbid();

            return File(file.Content, "application/octet-stream", file.Title);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile(FileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { message = "Не авторизован" });

            var file = new Core.Models.File
            {
                UserId = userId.Value,
                Title = dto.Title,
                Content = dto.Content,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Files.Add(file);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFile), new { id = file.Id }, new { file.Id, file.Title });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(int id, FileDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { message = "Не авторизован" });

            var file = await _context.Files.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == id);
            if (file == null) return NotFound(new { message = "Файл не найден" });

            bool isOwner = file.UserId == userId;
            bool isEditor = file.Permissions.Any(p => p.UserId == userId && p.PermissionType == "Editor");

            if (!isOwner && !isEditor)
                return Forbid();

            file.Title = dto.Title;
            file.Content = dto.Content;
            file.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Файл обновлён" });
        }

        [HttpPost("convert")]
        public IActionResult ConvertMarkdown([FromBody] string markdown)
        {
            var parser = new MarkdownParser();
            var tokens = parser.Parse(markdown);

            // Допустим, у тебя есть резолвер токенов в HTML
            var html = new TokenResolver().ResolveTokens(tokens); 

            return Ok(new { html });
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized(new { message = "Не авторизован" });

            var file = await _context.Files.FindAsync(id);
            if (file == null) return NotFound(new { message = "Файл не найден" });

            if (file.UserId != userId) return Forbid();

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Файл удалён" });
        }
    }
}
