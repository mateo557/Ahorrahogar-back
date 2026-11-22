using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var products = await _context.categorias.AsNoTracking().ToListAsync();
            return products.Select(ToDto);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var product = await _context.categorias.FindAsync(id);
            return product is null ? null : ToDto(product);
        }

        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto)
        {
            // DTO -> entidad
            var product = new Categorys
            {
                Nombre = dto.Nombre,
                Tipo = dto.Tipo,
            };

            _context.categorias.Add(product);
            await _context.SaveChangesAsync();

            return ToDto(product);
        }

        public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var product = await _context.categorias.FindAsync(id);
            if (product is null) return false;

            product.Nombre = dto.Nombre;
            product.Tipo = dto.Tipo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.categorias.FindAsync(id);
            if (product is null) return false;

            _context.categorias.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        // Entidad -> DTO (un solo lugar para no repetir codigo)
        private static CategoryDTO ToDto(Categorys p) => new()
        {
            Id= p.Id,
            Nombre = p.Nombre,
            Tipo = p.Tipo,
        };
    }
}
