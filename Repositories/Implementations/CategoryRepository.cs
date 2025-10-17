using Microsoft.EntityFrameworkCore;
using Sub_Pal_API.Data;
using Sub_Pal_API.Models;
using Sub_Pal_API.Repositories.Interfaces;

namespace Sub_Pal_API.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Categories
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAndUserIdAsync(string name, int userId)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == name && c.UserId == userId);
        }

        public async Task<Category> CreateAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
