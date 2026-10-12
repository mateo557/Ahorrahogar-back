using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services;

public class MovemntsService : IMovemntsService
{
    private readonly AppDbContext _context;

    public MovemntsService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<MovemntsDto>> GetAllAsync()
    {
        var products = await _context.movimientos.AsNoTracking().ToListAsync();
        return products.Select(ToDto);
    }

    public async Task<MovemntsDto?> GetByIdAsync(int id)
    {
        var product = await _context.movimientos.FindAsync(id);
        return product is null ? null : ToDto(product);
    }

    public async Task<MovemntsDto> CreateAsync(CreateMovemntsDto dto)
    {
        // DTO -> entidad
        var product = new Movemnts
        {
            Hogar_Id = dto.HogarId,
            Categoria_Id = dto.CategoriaId,
            Tipo = dto.Tipo,
            Fecha = dto.Fecha,
            Nota = dto.Nota,
        };

        _context.movimientos.Add(product);
        await _context.SaveChangesAsync();

        return ToDto(product);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMovemntsDto dto)
    {
        var product = await _context.movimientos.FindAsync(id);
        if (product is null) return false;

        
        product.Hogar_Id = dto.HogarId;
        product.Categoria_Id = dto.CategoriaId;
        product.Tipo = dto.Tipo;
        product.Fecha = dto.Fecha;
        product.Nota=dto.Nota;

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
    private static MovemntsDto ToDto(Movemnts p) => new()
    {
        HogarId = p.Hogar_Id,
        CategoriaId = p.Categoria_Id,
        Tipo = p.Tipo,
        Fecha = p.Fecha,
        Nota = p.Nota,
    };
}
