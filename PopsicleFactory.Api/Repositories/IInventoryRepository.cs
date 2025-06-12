using PopsicleFactory.Api.Models;

namespace PopsicleFactory.Api.Repositories;

public interface IInventoryRepository
{
    public Task<IEnumerable<PopsicleModel>> GetAllPopsicles();
    public Task<PopsicleModel?> GetPopsicleByIdAsync(int id);
    public Task<IEnumerable<PopsicleModel>> SearchPopsiclesAsync(string searchTerm);
    public Task<PopsicleModel> CreatePopsicleAsync(PopsicleModel popsicle);
    public Task<PopsicleModel> UpdatePopsicleAsync(PopsicleModel popsicle);
    public Task<bool> DeletePopsicleAsync(int id);
}