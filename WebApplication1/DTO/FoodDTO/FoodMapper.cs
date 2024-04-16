using OrderRestaurant.Data;
using OrderRestaurant.Model;

namespace OrderRestaurant.DTO.FoodDTO
{
    public static class FoodMapper
    {
        public static FoodModel ToFoodDto(this Food model)
        {
            return new FoodModel
            {
                FoodId = model.FoodId,
                NameFood = model.NameFood,
                UnitPrice = model.UnitPrice,
                UrlImage = model.UrlImage,
                CategoryId = model.CategoryId,

            };
        }

    }
}
