using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OrderRestaurant.Responsitory
{
    public class FoodResponsitory : IFood, ICommon<FoodModel>
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
                UrlImage = food.ImageFile
            };

           /* if (food.ImageFile != null && food.ImageFile.Length > 0)
            {
                var imagePath = "image";
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(food.ImageFile.FileName);
                var fullPath = Path.Combine(imagePath, imageName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await food.ImageFile.CopyToAsync(stream);
                }

                model.UrlImage = fullPath;*/
                _context.Foods.Add(model);
                await _context.SaveChangesAsync();
            
            return new Food
            {
                FoodId = model.FoodId,
                NameFood = model.NameFood,
                CategoryId = food.CategoryId,
                UnitPrice = model.UnitPrice,
                UrlImage = model.UrlImage,
               
            };
        }

        public async Task<FoodModel> GetFoodByIdAsync(int id)
        {
            var model = await _context.Foods
        .Select(s => new FoodModel
        {
            FoodId = s.FoodId,
            NameFood = s.NameFood,
            UnitPrice = s.UnitPrice,
            UrlImage = s.UrlImage,
            CategoryId = s.CategoryId,
            Category = _context.Categoies.FirstOrDefault(a => a.CategoryId == s.CategoryId)
        })
        .Where(a => a.FoodId == id)
        .FirstOrDefaultAsync();

            return model;

        }

        public Task<bool> FoodExits(int id)
        {
            return _context.Foods.AnyAsync(s => s.FoodId == id);
        }

        public async Task<Food> UpdateFood(int id, UpdateFoodDTO updateFoodDTO)
        {
            var updateFood = await _context.Foods.FirstOrDefaultAsync(hh => hh.FoodId == id);
            if (updateFood == null)
            {
                return null;
            }
            updateFood.NameFood = updateFoodDTO.NameFood;
            updateFood.UnitPrice = updateFoodDTO.UnitPrice;
            updateFood.CategoryId = updateFoodDTO.CategoryId;
            updateFood.UrlImage = updateFoodDTO.Image;
           /* // Xử lý ảnh
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
            }*/

            await _context.SaveChangesAsync();
            return updateFood;
        }

        public async Task<Food?> DeleteFood(int id)
        {
            var model = await _context.Foods.FirstOrDefaultAsync(i => i.FoodId == id);
            if (model == null)
            {
                return null;

            }
            _context.Foods.Remove(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<(int totalItems, int totalPages, List<Food> foods)> GetSearchFood(QuerryFood querry, string search = "")
        {
            var query = _context.Foods
                .Include(f => f.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(f => EF.Functions.Like(f.NameFood, $"%{search}%"));
            }
            if (querry.CategoryId != null)
            {
                query = query.Where(s => s.CategoryId == querry.CategoryId);
            }
            if (!string.IsNullOrWhiteSpace(querry.SortBy))
            {
                if (querry.SortBy.Equals("GiamDan", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderByDescending(s => s.UnitPrice);
                }
                else if (querry.SortBy.Equals("TangDan", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderBy(s => s.UnitPrice);
                }
            }
            var totalItems = await query.CountAsync(); // Số lượng sản phẩm tìm kiếm được
            var totalPages = (int)Math.Ceiling((double)totalItems / querry.PageSize); // Số trang
            var skipNumber = (querry.PageNumber - 1) * querry.PageSize;
            var foods = await query
                .Select(f => new Food
                {
                    FoodId = f.FoodId,
                    NameFood = f.NameFood,
                    UnitPrice = f.UnitPrice,
                    UrlImage = f.UrlImage,
                    CategoryId = f.CategoryId,
                    Category = f.Category
                })
                .Skip(skipNumber)
                .Take(querry.PageSize)
                .ToListAsync();

            return (totalItems, totalPages, foods);
        }

        public async Task<(int totalItems, int totalPages, List<FoodModel> items)> SearchAndPaginate(QuerryObject querryObject)
        {
            var query = _context.Foods
                                .Include(f=>f.Category)
                                .AsQueryable();

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrWhiteSpace(querryObject.Search))
            {
                query = query.Where(f => EF.Functions.Like(f.NameFood, $"%{querryObject.Search}%"));
            }

            // Tính toán số trang và lấy dữ liệu phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / querryObject.PageSize);
            var skipNumber = (querryObject.PageNumber - 1) * querryObject.PageSize;
            var items = await query.Skip(skipNumber).Take(querryObject.PageSize).ToListAsync();
            var foods = await query
                .Select(f => new FoodModel
                {
                    FoodId = f.FoodId,
                    NameFood = f.NameFood,
                    UnitPrice = f.UnitPrice,
                    UrlImage = f.UrlImage,
                    CategoryId = f.CategoryId,
                    Category = _context.Categoies
                                   .Where(a => a.CategoryId == f.CategoryId )
                                   .Select(o => new Category
                                   {
                                       CategoryId = o.CategoryId,
                                       CategoryName = o.CategoryName,
                                       Description = o.Description,
                                   })
                                   .FirstOrDefault(),
                })
                .Skip(skipNumber)
                .Take(querryObject.PageSize)
                .ToListAsync();
            return (totalItems, totalPages, foods);
        }
    }
}
