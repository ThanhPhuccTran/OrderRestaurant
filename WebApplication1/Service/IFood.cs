using OrderRestaurant.Data;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;

namespace OrderRestaurant.Service
{
    public interface IFood
    {
        Task<List<Food>> GetFilterFood(QuerryFood querry);
        Task<Food> CreateFoodAsync(CreateFoodDTO food);
        Task<FoodModel> GetFoodByIdAsync(int id);
        Task<List<FoodModel>> GetFoodByCategory(int categoryid);
        Task<List<Food>> GetAllFoods();
        Task<Food>UpdateFood (int id, UpdateFoodDTO updateFoodDTO);
        Task<Food?> DeleteFood(int id);
        Task<bool> FoodExits(int id);
    }
}
