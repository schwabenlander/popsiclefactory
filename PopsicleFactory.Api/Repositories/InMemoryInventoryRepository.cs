using PopsicleFactory.Api.Models;

namespace PopsicleFactory.Api.Repositories;

public class InMemoryInventoryRepository : IInventoryRepository
{
    private readonly List<PopsicleModel> _popsicles;
    private int _nextId;

    public InMemoryInventoryRepository()
    {
        _popsicles = new List<PopsicleModel>
        {
            new PopsicleModel { Id = 1, Name = "Strawberry Delight", Description = "Fresh strawberry popsicle with real fruit chunks" },
            new PopsicleModel { Id = 2, Name = "Blueberry Bliss", Description = "Delicious blueberry popsicle with a hint of lemon" },
            new PopsicleModel { Id = 3, Name = "Orange Creamsicle", Description = "Creamy orange popsicle with vanilla center" }
        };
        _nextId = 4;
    }

    public Task<IEnumerable<PopsicleModel>> GetAllPopsicles()
    {
        return Task.FromResult(_popsicles.AsEnumerable());
    }

    public Task<PopsicleModel?> GetPopsicleByIdAsync(int id)
    {
        var popsicle = _popsicles.FirstOrDefault(p => p.Id == id);
        return Task.FromResult(popsicle);
    }

    public Task<IEnumerable<PopsicleModel>> SearchPopsiclesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return Task.FromResult(_popsicles.AsEnumerable());
        }

        var results = _popsicles.Where(p =>
            p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        
        return Task.FromResult(results);
    }

    public Task<PopsicleModel> CreatePopsicleAsync(PopsicleModel popsicle)
    {
        popsicle.Id = _nextId++;
        _popsicles.Add(popsicle);
        return Task.FromResult(popsicle);
    }

    public Task<PopsicleModel> UpdatePopsicleAsync(PopsicleModel popsicle)
    {
        var existingIndex = _popsicles.FindIndex(p => p.Id == popsicle.Id);
        if (existingIndex >= 0)
        {
            _popsicles[existingIndex] = popsicle;
        }
        return Task.FromResult(popsicle);
    }

    public Task<bool> DeletePopsicleAsync(int id)
    {
        var popsicle = _popsicles.FirstOrDefault(p => p.Id == id);
        if (popsicle != null)
        {
            _popsicles.Remove(popsicle);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}