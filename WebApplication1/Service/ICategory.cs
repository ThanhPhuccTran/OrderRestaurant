using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.Helpers;

namespace OrderRestaurant.Service
{
    public interface ICategory
    {
        Task<( int totalItems,int totalPages,List<Category> category )> GetSearch(QuerryObject querry , string search = "");
        Task<List<Category>> GetCategoryFoods();
        Task<Category?> GetCategoryFoodById(int id);
        Task<Category> CreateCategory(Category categoryFood);
        Task<Category> UpdateCategory(int id, UpdateCategoryDTO updateCategoryDTO);
        Task<Category?> DeleteCategory(int id);
        Task<bool> CategoryExit(int id);
    }
}
