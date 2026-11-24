using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services;

public class MovemntsService : IMovemntsService
{
    private readonly AppDbContext _context;

    public MovemntsService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<MovemntsDto>> GetAllAsync(int hogarId, int? anio = null, int? mes = null)
    {
        var query = _context.movimientos.AsNoTracking().Where(m => m.hogar_id == hogarId);

        if (anio.HasValue && mes.HasValue)
        {
            var desde = new DateOnly(anio.Value, mes.Value, 1);
            var hasta = desde.AddMonths(1);

            query = query.Where(m => m.Fecha >= desde && m.Fecha < hasta);
        }

        var movimientos = await query.OrderByDescending(m => m.Fecha).ToListAsync();
        return movimientos.Select(ToDto);
    }

    public async Task<MovemntsDto?> GetByIdAsync(int id, int hogarId)
    {
        var mov = await _context.movimientos
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.hogar_id == hogarId);
        return mov is null ? null : ToDto(mov);
    }

    public async Task<MovemntsDto> CreateAsync(int hogarId, CreateMovemntsDto dto)
    {
        var categoria = await _context.categorias.FindAsync(dto.categoria_id)
            ?? throw new KeyNotFoundException("Categoría no existe");

        var product = new Movemnts
        {
            hogar_id = hogarId, // el hogar lo pone el servidor, no la app
            categoria_id = dto.categoria_id,
            tipo = categoria.Tipo,
            monto = dto.monto,
            Fecha = dto.Fecha,
            Nota = dto.Nota,
            Creado_En = DateTime.Now
        };

        _context.movimientos.Add(product);

        await ApplyToBalanceAsync(product.hogar_id, product.Fecha, categoria.Tipo, product.monto);

        await _context.SaveChangesAsync();

        return ToDto(product);
    }

    public async Task<bool> UpdateAsync(int id, int hogarId, UpdateMovemntsDto dto)
    {
        var mov = await _context.movimientos.FirstOrDefaultAsync(m => m.Id == id && m.hogar_id == hogarId);
        if (mov is null) return false;

        // Se valida la categoría ANTES de tocar el balance; si no existe, el balance no queda a medias
        var categoria = await _context.categorias.FindAsync(dto.categoria_id)
            ?? throw new KeyNotFoundException("Categoría no existe");

        // 1. quita el efecto viejo (en su mes original)
        await ApplyToBalanceAsync(mov.hogar_id, mov.Fecha, mov.tipo, -mov.monto);

        // 2. aplica los datos nuevos
        mov.categoria_id = dto.categoria_id;
        mov.tipo = categoria.Tipo;
        mov.monto = dto.monto;
        mov.Fecha = dto.Fecha;
        mov.Nota = dto.Nota;

        // 3. suma el efecto nuevo (puede ser otro mes u otra categoría)
        await ApplyToBalanceAsync(mov.hogar_id, mov.Fecha, mov.tipo, mov.monto);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int hogarId)
    {
        var product = await _context.movimientos.FirstOrDefaultAsync(m => m.Id == id && m.hogar_id == hogarId);
        if (product is null) return false;

        await ApplyToBalanceAsync(product.hogar_id, product.Fecha, product.tipo, -product.monto); // revierte
        _context.movimientos.Remove(product);

        await _context.SaveChangesAsync();
        return true;
    }


    // monto positivo suma al balance, monto negativo lo revierte
    private async Task ApplyToBalanceAsync(int hogarId, DateOnly fecha, string tipo, decimal monto)
    {
        var periodo = new DateOnly(fecha.Year, fecha.Month, 1);

        var balance = await _context.balances
            .FirstOrDefaultAsync(b => b.hogar_id == hogarId && b.periodo == periodo);

        if (balance is null)
        {
            balance = new Balance { hogar_id = hogarId, periodo = periodo };
            _context.balances.Add(balance);          // no existe ese mes: lo crea
        }

        if (tipo == "ingreso") balance.balance_ingreso += monto;
        else balance.balance_gasto += monto;   // gasto en positivo

        balance.balance = balance.balance_ingreso - balance.balance_gasto;

        await _context.SaveChangesAsync();   // quítalo si quien llama ya guarda
    }

    // Entidad -> DTO (un solo lugar para no repetir codigo)
    private static MovemntsDto ToDto(Movemnts p) => new()
    {
        Id = p.Id,
        categoria_id = p.categoria_id,
        monto = p.monto,
        Fecha = p.Fecha,
        Nota = p.Nota,
        creado_en = p.Creado_En
    };
}
