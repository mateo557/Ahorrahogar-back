using AhorrahogarApi.DTOs;

namespace AhorrahogarApi.Services;

public interface IMovemntsService
{
    Task<IEnumerable<MovemntsDto>> GetAllAsync();
    Task<MovemntsDto?> GetByIdAsync(int id);
    Task<MovemntsDto> CreateAsync(CreateMovemntsDto dto);
    Task<bool> UpdateAsync(int id, UpdateMovemntsDto dto);
    Task<bool> DeleteAsync(int id);
}
