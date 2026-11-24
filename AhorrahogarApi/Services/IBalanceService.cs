using AhorrahogarApi.DTOs;

namespace AhorrahogarApi.Services
{
    public interface IBalanceService
    {
        Task<IEnumerable<BalanceDTO>> GetAllAsync();
        Task<BalanceDTO?> GetByIdAsync(int hogarId);

        Task<BalanceDTO?> GetByPeriodoAsync(int hogarId, int year, int month);

        Task<IEnumerable<CategoriaTotalDto>> GetGastosPorCategoriaAsync(int hogarId, int anio, int mes);
    }


}
