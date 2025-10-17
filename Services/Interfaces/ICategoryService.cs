using Sub_Pal_API.Models;

namespace Sub_Pal_API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync(int userId);
        Task<Category> EnsureCategoryExistsAsync(string categoryName, int userId);
    }
}
