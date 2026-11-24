using AhorrahogarApi.Data;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AhorrahogarApi.Services
{
    public class BalanceService:IBalanceService
    {
        private readonly AppDbContext _context;

        public BalanceService(AppDbContext context) => _context = context;

        public async Task<IEnumerable<BalanceDTO>> GetAllAsync()
        {
            var products = await _context.balances.AsNoTracking().ToListAsync();
            return products.Select(ToDto);
        }

     

        public async Task<BalanceDTO?> GetByIdAsync(int id)
        {
            var product = await _context.balances.FindAsync(id);
            return product is null ? null : ToDto(product); 

        }

     

        public async Task<BalanceDTO?> GetByPeriodoAsync(int hogarId, int year, int month)
        {
            var periodo = new DateOnly(year, month, 1);

            return await _context.balances
                .Where(b => b.hogar_id == hogarId && b.periodo == periodo)
                .Select(b => new BalanceDTO
                {
                    id=b.id,
                    hogar_id=b.hogar_id,
                    periodo = b.periodo,
                    balance_ingreso = b.balance_ingreso,
                    balance_gasto = b.balance_gasto,
                    //Balance = b.TotalIngresos - b.TotalGastos   // se calcula, no se guarda
                    balance=b.balance,
                })
                .FirstOrDefaultAsync();
        }

        private static BalanceDTO ToDto(Balance p) => new()
        {
            id=p.id,
            periodo=p.periodo,
            hogar_id = p.hogar_id,
            balance = p.balance,
            balance_ingreso = p.balance_ingreso,
            balance_gasto = (p.balance_gasto) * -1,

        };

        public async Task<IEnumerable<CategoriaTotalDto>> GetGastosPorCategoriaAsync(int hogarId, int anio, int mes)
        {
            var desde = new DateOnly(anio, mes, 1);
            var hasta = desde.AddMonths(1);

            var gastos = await _context.movimientos
                .AsNoTracking()
                .Where(m => m.hogar_id == hogarId && m.Fecha >= desde && m.Fecha < hasta)
                .Join(_context.categorias.Where(c => c.Tipo == "gasto"),
                      m => m.categoria_id, c => c.Id,
                      (m, c) => new { c.Id, c.Nombre, m.monto })
                .GroupBy(x => new { x.Id, x.Nombre })
                .Select(g => new { g.Key.Id, g.Key.Nombre, Total = g.Sum(x => x.monto) })
                .ToListAsync();

            var totalGastos = gastos.Sum(g => g.Total);

            return gastos
                .OrderByDescending(g => g.Total)
                .Select(g => new CategoriaTotalDto
                {
                    categoria_id = g.Id,
                    nombre = g.Nombre,
                    total = g.Total,
                    porcentaje = totalGastos > 0 ? Math.Round(g.Total / totalGastos * 100, 1) : 0
                });
        }
    }
}
