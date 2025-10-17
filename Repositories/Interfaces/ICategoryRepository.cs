using Sub_Pal_API.Models;

namespace Sub_Pal_API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllByUserIdAsync(int userId);
        Task<Category?> GetByNameAndUserIdAsync(string name, int userId);
        Task<Category> CreateAsync(Category category);
    }
}
