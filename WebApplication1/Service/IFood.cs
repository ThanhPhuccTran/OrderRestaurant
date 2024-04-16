using OrderRestaurant.Data;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.Model;

namespace OrderRestaurant.Service
{
    public interface IFood
    {
        Task<Food> CreateFoodAsync(CreateFoodDTO food);
        Task<FoodModel> GetFoodByIdAsync(int id);
        Task<List<Food>> GetAllFoods();
        Task<Food>UpdateFood (int id, UpdateFoodDTO updateFoodDTO);
        Task<Food?> DeleteFood(int id);
        Task<bool> FoodExits(int id);
    }
}
