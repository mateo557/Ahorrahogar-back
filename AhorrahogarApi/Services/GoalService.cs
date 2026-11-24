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

        public async Task<IEnumerable<GoalsDto>> GetAllAsync(int hogarId)
        {
            var metas = await _context.metas
                .AsNoTracking()
                .Where(g => g.Hogar_Id == hogarId)
                .OrderBy(g => g.fechalimite)
                .ThenBy(g => g.creadoen)
                .ToListAsync();

            var totalAhorrado = await _context.balances
                .Where(b => b.hogar_id == hogarId)
                .SumAsync(b => (decimal?)b.balance) ?? 0m;

            var disponible = Math.Max(totalAhorrado, 0m);
            var resultado = new List<GoalsDto>();

            foreach (var meta in metas)
            {
                var asignado = Math.Min(disponible, meta.montoobjetivo);
                disponible -= asignado;

                var dto = ToDto(meta);
                dto.montoahorrado = asignado;
                dto.porcentaje = meta.montoobjetivo > 0
                    ? Math.Round(asignado / meta.montoobjetivo * 100, 1)
                    : 0;
                resultado.Add(dto);
            }

            return resultado;
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
                montoobjetivo=dto.montoobjetivo,
                fechalimite=dto.fechalimite
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
            product.montoobjetivo = dto.montoobjetivo;
            product.fechalimite = dto.fechalimite;

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
            id=p.Id,
            Hogar_Id = p.Hogar_Id,
            Nombre = p.Nombre,
            montoobjetivo=p.montoobjetivo,
            fechalimite = p.fechalimite
            
    };
    }
}
