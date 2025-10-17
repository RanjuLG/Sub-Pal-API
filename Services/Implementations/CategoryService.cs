using Sub_Pal_API.Models;
using Sub_Pal_API.Repositories.Interfaces;
using Sub_Pal_API.Services.Interfaces;

namespace Sub_Pal_API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<List<Category>> GetAllCategoriesAsync(int userId)
        {
            return await _categoryRepository.GetAllByUserIdAsync(userId);
        }

        public async Task<Category> EnsureCategoryExistsAsync(string categoryName, int userId)
        {
            // Check if category already exists for this user
            var existingCategory = await _categoryRepository.GetByNameAndUserIdAsync(categoryName, userId);
            
            if (existingCategory != null)
            {
                return existingCategory;
            }

            // Create new category if it doesn't exist
            var newCategory = new Category
            {
                Name = categoryName,
                UserId = userId
            };

            return await _categoryRepository.CreateAsync(newCategory);
        }
    }
}
