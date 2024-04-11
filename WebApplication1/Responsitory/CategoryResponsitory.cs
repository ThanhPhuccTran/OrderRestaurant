using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class CategoryResponsitory : ICategory
    {
        private readonly ApplicationDBContext _context;
        public CategoryResponsitory(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateCategory(Category categoryFood)
        {
            await _context.Categoies.AddAsync(categoryFood);
            await _context.SaveChangesAsync();
            return categoryFood;
        }

        public async Task<Category?> DeleteCategory(int id)
        {
            var model = await _context.Categoies.FirstOrDefaultAsync(i => i.CategoryId == id);
            if (model == null)
            {
                return null;
            }
            _context.Categoies.Remove(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<Category?> GetCategoryFoodById(int id)
        {
            return await _context.Categoies.FindAsync(id);
        }

        public async Task<List<Category>> GetCategoryFoods()
        {
            return await _context.Categoies.ToListAsync();
        }

        public async Task<Category> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDTO)
        {
            var categoryupdate = await _context.Categoies.FirstOrDefaultAsync(hh => hh.CategoryId == id);
            if (categoryupdate == null)
            {
                return null;
            }

            categoryupdate.TenLoai = updateCategoryDTO.TenLoai;

            await _context.SaveChangesAsync();
            return categoryupdate;
        }
    }
}
