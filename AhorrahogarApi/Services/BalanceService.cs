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

     

        public async Task<BalanceDTO?> GetByIdAsync(int hogarId)
        {
            var product = await _context.balances.FindAsync(hogarId);
            return product is null ? null : ToDto(product); 

        }

        private static BalanceDTO ToDto(Balance p) => new()
        {
            hogar_id = p.hogar_id,
            balance = p.balance,
            balance_ingreso = p.balance_ingreso,
            balance_gasto = (p.balance_gasto)*-1,
            
        };
    }
}
