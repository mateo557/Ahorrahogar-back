using AhorrahogarApi.DTOs;

namespace AhorrahogarApi.Services;

// Todas las operaciones reciben el hogar de quien hace la petición:
// un hogar nunca puede ver ni tocar los movimientos de otro.
public interface IMovemntsService
{
    Task<IEnumerable<MovemntsDto>> GetAllAsync(int hogarId, int? anio = null, int? mes = null);
    Task<MovemntsDto?> GetByIdAsync(int id, int hogarId);
    Task<MovemntsDto> CreateAsync(int hogarId, CreateMovemntsDto dto);
    Task<bool> UpdateAsync(int id, int hogarId, UpdateMovemntsDto dto);
    Task<bool> DeleteAsync(int id, int hogarId);
}
