using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services
{
    public class GoalService : IGoalService
    {
        private readonly AppDbContext _context;

        public GoalService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<GoalsDto>> GetAllAsync()
        {
            var products = await _context.metas.AsNoTracking().ToListAsync();
            return products.Select(ToDto);
        }

        public async Task<GoalsDto?> GetByIdAsync(int id)
        {
            var product = await _context.metas.FindAsync(id);
            return product is null ? null : ToDto(product);
        }

        public async Task<GoalsDto> CreateAsync(CreateGoalsDto dto)
        {
            // DTO -> entidad
            var product = new Goals
            {
                Hogar_Id = dto.Hogar_Id,
                Nombre = dto.Nombre,
                Creado_En = DateTime.Now
            };

            _context.metas.Add(product);
            await _context.SaveChangesAsync();

            return ToDto(product);
        }

        public async Task<bool> UpdateAsync(int id, UpdateGoalsDto dto)
        {
            var product = await _context.metas.FindAsync(id);
            if (product is null) return false;


            product.Hogar_Id = dto.Hogar_Id;
            product.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.metas.FindAsync(id);
            if (product is null) return false;

            _context.metas.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // Entidad -> DTO (un solo lugar para no repetir codigo)
        private static GoalsDto ToDto(Goals p) => new()
        {
            Hogar_Id = p.Hogar_Id,
            Nombre = p.Nombre,
            Creado_En = p.Creado_En
        };
    }
}
