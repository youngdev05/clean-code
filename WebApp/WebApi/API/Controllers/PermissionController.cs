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
    [Route("api/files/{fileId}/permissions")]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PermissionsController(AppDbContext context)
        {
            _context = context;
        }

        // Назначить права пользователю
        [HttpPost]
        public async Task<IActionResult> SetPermission(int fileId, PermissionDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var file = await _context.Files.FindAsync(fileId);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            if (file.UserId != userId) return Forbid();

            var permission = new FilePermission
            {
                FileId = fileId,
                UserId = dto.UserId,
                PermissionType = dto.PermissionType
            };

            _context.FilePermissions.Add(permission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Права обновлены" });
        }

        // Удалить права
        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemovePermission(int fileId, int userId)
        {
            var ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var file = await _context.Files.Include(f => f.Permissions).FirstOrDefaultAsync(f => f.Id == fileId);

            if (file == null) return NotFound(new { message = "Файл не найден" });

            if (file.UserId != ownerId) return Forbid();

            var permission = file.Permissions.FirstOrDefault(p => p.UserId == userId);
            if (permission == null) return NotFound(new { message = "Права не найдены" });

            _context.FilePermissions.Remove(permission);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Права удалены" });
        }
    }
}
