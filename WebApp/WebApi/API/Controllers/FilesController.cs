using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.API.DTOs;
using WebApi.Persistence;
using WebApi.Core.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize] // Все эндпоинты требуют аутентификации
    public class FilesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilesController(AppDbContext context)
        {
            _context = context;
        }

        // Получить список файлов (доступные пользователю)
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

        // Получить содержимое файла
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var file = await _context.Files.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == id);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            // Проверяем, есть ли у пользователя права на просмотр
            if (file.UserId != userId && !file.Permissions.Any(p => p.UserId == userId))
                return Forbid();

            return Ok(new { file.Id, file.Title, file.Content, file.UpdatedAt });
        }

        // Создать файл
        [HttpPost]
        public async Task<IActionResult> CreateFile(FileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var file = new WebRequestMethods.File
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

        // Обновить файл (только владелец или Editor)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFile(int id, FileDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var file = await _context.Files.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == id);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            // Проверяем, имеет ли пользователь право редактирования
            if (file.UserId != userId && !file.Permissions.Any(p => p.UserId == userId && p.PermissionType == "Editor"))
                return Forbid();

            file.Title = dto.Title;
            file.Content = dto.Content;
            file.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Файл обновлён" });
        }

        // Удалить файл (только владелец)
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
