using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

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
            hogar_id = dto.hogar_id,
            categoria_id = dto.categoria_id,
            monto=dto.monto,
            Fecha = dto.Fecha,
            Nota = dto.Nota,
            Creado_En = DateTime.Now
        };

        _context.movimientos.Add(product);
        var category = await _context.categorias.FindAsync(dto.categoria_id);
        bool Type = category.Tipo.ToLower() == "ingreso";
        

        var balanceRecord = await _context.balances.FindAsync(dto.hogar_id);

        if (Type)
        {
            balanceRecord.balance_ingreso += dto.monto;
            balanceRecord.balance += dto.monto;
        }
        else
        {
            balanceRecord.balance_gasto -= dto.monto;
            balanceRecord.balance -= dto.monto;
        }

        await _context.SaveChangesAsync();

        return ToDto(product);
    }

    public async Task<bool> UpdateAsync(int id, UpdateMovemntsDto dto)
    {
        var movimiento = await _context.movimientos.FindAsync(id);
        if (movimiento is null) return false;

        var oldCategory = await _context.categorias.FindAsync(movimiento.categoria_id);
        var oldingreso = await _context.balances.FindAsync(movimiento.hogar_id);
        var newingreso = await _context.balances.FindAsync(dto.hogar_id);
        var oldgasto = await _context.balances.FindAsync(movimiento.hogar_id);
        var newgasto = await _context.balances.FindAsync(dto.hogar_id);
        var newCategory = await _context.categorias.FindAsync(dto.categoria_id);
        var oldBalance = await _context.balances.FindAsync(movimiento.hogar_id);
        var newBalance = await _context.balances.FindAsync(dto.hogar_id);

        if (oldCategory is null || newCategory is null ||
            oldBalance is null || newBalance is null || 
            oldgasto is null || newgasto is null || 
            newingreso is null || oldingreso is null) return false;

        // 1. Revertir el movimiento viejo
        if (oldCategory.Tipo.ToLower() == "ingreso")
            oldBalance.balance -= movimiento.monto;
        else
            oldBalance.balance += movimiento.monto;

        if (oldCategory.Tipo.ToLower() == "ingreso")
            oldingreso.balance -= movimiento.monto;
        else
            oldgasto.balance += movimiento.monto;

        // 2. Aplicar el movimiento nuevo
        if (newCategory.Tipo.ToLower() == "ingreso")
            newBalance.balance += dto.monto;
        else
            newBalance.balance -= dto.monto;

        if (newCategory.Tipo.ToLower() == "ingreso")
            newingreso.balance += dto.monto;
        else
            newgasto.balance -= dto.monto;

        // 3. Actualizar los campos
        movimiento.hogar_id = dto.hogar_id;
        movimiento.categoria_id = dto.categoria_id;
        movimiento.monto =dto.monto;
        movimiento.Fecha = dto.Fecha;
        movimiento.Nota = dto.Nota;

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
        categoria_id = p.categoria_id,
        monto = p.monto,
        Fecha = p.Fecha,
        Nota = p.Nota,
        creado_en = p.Creado_En
    };
}
