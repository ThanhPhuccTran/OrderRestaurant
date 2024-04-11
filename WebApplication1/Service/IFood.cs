using OrderRestaurant.Data;
using OrderRestaurant.DTO.FoodDTO;

namespace OrderRestaurant.Service
{
    public interface IFood
    {
        Task<Food> CreateFoodAsync(CreateFoodDTO food);
        Task<Food> GetFoodByIdAsync(int id);
        Task<List<Food>> GetAllFoods();
        Task<Food>UpdateFood (int id, UpdateFoodDTO updateFoodDTO);
        Task<Food?> DeleteFood(int id);
        Task<bool> FoodExits(int id);
    }
}
