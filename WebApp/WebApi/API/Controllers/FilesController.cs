using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Persistence;
using WebApi.Core.Models;
using WebApi.API.DTOs;

namespace API.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize] 
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var files = await _context.Files
                .Where(f => f.UserId == userId || f.Permissions.Any(p => p.UserId == userId))
                .Select(f => new { f.Id, f.Title, Owner = f.Owner.Username })
                .ToListAsync();

            return Ok(files);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var file = await _context.Files.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == id);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            if (file.UserId != userId && !file.Permissions.Any(p => p.UserId == userId))
                return Forbid();

            return Ok(new { file.Id, file.Title, file.Content, file.UpdatedAt });
        }

        [HttpPost]
        public async Task<IActionResult> CreateFile(FileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var file = new WebApi.Core.Models.File 
            { 
                UserId = userId, 
                Title = dto.Title, 
                Content = dto.Content, 
                UpdatedAt = DateTime.UtcNow 
            };

            _context.Files.Add(file);
            await _context.SaveChangesAsync();

            return Ok(new { file.Id, file.Title });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(int id, FileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var file = await _context.Files.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == id);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            if (file.UserId != userId && !file.Permissions.Any(p => p.UserId == userId && p.PermissionType == "Editor"))
                return Forbid();

            file.Title = dto.Title;
            file.Content = dto.Content;
            file.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Файл обновлён" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var file = await _context.Files.FindAsync(id);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            if (file.UserId != userId) return Forbid();

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Файл удалён" });
        }
    }
}
