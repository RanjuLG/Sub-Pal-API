using Sub_Pal_API.Models;
using Sub_Pal_API.Repositories.Interfaces;
using Sub_Pal_API.Services.Interfaces;

namespace Sub_Pal_API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICacheService _cacheService;

        public CategoryService(ICategoryRepository categoryRepository, ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _cacheService = cacheService;
        }

        private string GetCategoriesCacheKey(int userId) => $"user:{userId}:categories";

        public async Task<List<Category>> GetAllCategoriesAsync(int userId)
        {
            var cacheKey = GetCategoriesCacheKey(userId);
            
            // Try to get from cache
            var cachedCategories = await _cacheService.GetAsync<List<Category>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            // Get from database and cache
            var categories = await _categoryRepository.GetAllByUserIdAsync(userId);
            await _cacheService.SetAsync(cacheKey, categories, TimeSpan.FromMinutes(30));
            
            return categories;
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

            var result = await _categoryRepository.CreateAsync(newCategory);
            
            // Invalidate categories cache
            await _cacheService.RemoveAsync(GetCategoriesCacheKey(userId));
            
            return result;
        }
    }
}
