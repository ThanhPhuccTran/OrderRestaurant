using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;

namespace OrderRestaurant.Service
{
    public interface ICategory
    {
        Task<List<Category>> GetCategoryFoods();
        Task<Category?> GetCategoryFoodById(int id);
        Task<Category> CreateCategory(Category categoryFood);
        Task<Category> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDTO);
        Task<Category?> DeleteCategory(int id);
    }
}
