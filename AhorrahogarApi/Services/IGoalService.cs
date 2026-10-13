using AhorrahogarApi.DTOs;

namespace AhorrahogarApi.Services
{
    public interface IGoalService
    {
        Task<IEnumerable<GoalsDto>> GetAllAsync();
        Task<GoalsDto?> GetByIdAsync(int id);
        Task<GoalsDto> CreateAsync(CreateGoalsDto dto);
        Task<bool> UpdateAsync(int id, UpdateGoalsDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
