using deneme.Models;

namespace deneme.Repositories
{
    public interface IErasmusProgramiRepository
    {
        Task<ErasmusProgrami?> GetByIdAsync(int id);
        Task<List<ErasmusProgrami>> GetAllAsync();
        Task<List<ErasmusProgrami>> SearchAsync(string searchTerm);
        Task<ErasmusProgrami> AddAsync(ErasmusProgrami program);
        Task UpdateAsync(ErasmusProgrami program);
        Task DeleteAsync(int id);
    }
}

