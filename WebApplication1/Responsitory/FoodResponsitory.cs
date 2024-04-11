using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class FoodResponsitory : IFood
    {
        private readonly ApplicationDBContext _context;
        public FoodResponsitory(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<List<Food>> GetAllFoods()
        {
            return await _context.Foods.ToListAsync();
        }
        public async Task<Food> CreateFoodAsync(CreateFoodDTO food)
        {
            var model = new Food
            {
                NameFood = food.NameFood,
                UnitPrice = food.UnitPrice,
                CategoryId = food.CategoryId,
            };

            if (food.ImageFile != null && food.ImageFile.Length > 0)
            {
                var imagePath = "image";
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(food.ImageFile.FileName);
                var fullPath = Path.Combine(imagePath, imageName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await food.ImageFile.CopyToAsync(stream);
                }

                model.UrlImage = fullPath;
                _context.Foods.Add(model);
                await _context.SaveChangesAsync();
            }
            return new Food
            {
                FoodId = model.FoodId,
                NameFood = model.NameFood,
                UnitPrice = model.UnitPrice,
                UrlImage = model.UrlImage,
                CategoryId = food.CategoryId
            };
        }

        public async Task<Food> GetFoodByIdAsync(int id)
        {
            return await _context.Foods.FindAsync(id);
        }

        public Task<bool> FoodExits(int id)
        {
            return _context.Foods.AnyAsync(s => s.FoodId == id);
        }

        public async Task<Food> UpdateFood(int id, UpdateFoodDTO updateFoodDTO)
        {
            var updateFood = await _context.Foods.FirstOrDefaultAsync(hh => hh.FoodId == id);
            if(updateFood == null)
            {
                return null;
            }
            updateFood.NameFood = updateFoodDTO.NameFood;
            updateFood.UnitPrice = updateFoodDTO.UnitPrice;
            updateFood.CategoryId = updateFoodDTO.CategoryId;
            // Xử lý ảnh
            if (updateFoodDTO.Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    updateFoodDTO.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    updateFood.UrlImage = base64String;
                }
            }
            else
            {
                updateFood.UrlImage = "";
            }
            
            await _context.SaveChangesAsync();
            return updateFood;
        }

        public Task<Food?> DeleteFood(int id)
        {
            throw new NotImplementedException();
        }
    }
}
