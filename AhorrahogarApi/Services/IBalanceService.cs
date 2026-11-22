using AhorrahogarApi.DTOs;

namespace AhorrahogarApi.Services
{
    public interface IBalanceService
    {
        Task<IEnumerable<BalanceDTO>> GetAllAsync();
        Task<BalanceDTO?> GetByIdAsync(int hogarId);
    }
}
