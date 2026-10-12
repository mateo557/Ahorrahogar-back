using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services
{
    
        public class MemberService : IMemberService
        {
            private readonly AppDbContext _context;

            public MemberService(AppDbContext context) => _context = context;

            public async Task<IEnumerable<MemberDto>> GetAllAsync()
            {
                var products = await _context.integrantes.AsNoTracking().ToListAsync();
                return products.Select(ToDto);
            }

            public async Task<MemberDto?> GetByIdAsync(int id)
            {
                var product = await _context.integrantes.FindAsync(id);
                return product is null ? null : ToDto(product);
            }

            public async Task<MemberDto> CreateAsync(CreateMemberDto dto)
            {
                // DTO -> entidad
                var product = new Members
                {
                    Hogar_Id = dto.Hogar_Id,
                    nombre = dto.nombre,
                    Creado_En = DateTime.Now
                };

                _context.integrantes.Add(product);
                await _context.SaveChangesAsync();

                return ToDto(product);
            }

            public async Task<bool> UpdateAsync(int id, UpdateMemberDto dto)
            {
                var product = await _context.integrantes.FindAsync(id);
                if (product is null) return false;


                product.Hogar_Id = dto.Hogar_Id;
                product.nombre = dto.nombre;

                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var product = await _context.movimientos.FindAsync(id);
                if (product is null) return false;

                _context.movimientos.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }

            // Entidad -> DTO (un solo lugar para no repetir codigo)
            private static MemberDto ToDto(Members p) => new()
            {
                Hogar_Id = p.Hogar_Id,
                nombre= p.nombre,
                Creado_En = p.Creado_En
            };
        }
}