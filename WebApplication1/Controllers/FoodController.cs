using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFood _foodRepository;
        private readonly ApplicationDBContext _context;
        private readonly ICategory _categoryRespository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment env;
        public FoodController(IFood foodRepository,ICategory categoryRespository, ApplicationDBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _foodRepository = foodRepository;
            _context = context;
            this.env = env;
            _categoryRespository = categoryRespository;
        }
        //https://localhost:7014/api/Food/search
        //Phân trang và tìm kiếm , tìm kiếm theo giá asc desc , tìm theo category
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] QuerryFood querry, string search = "")
        {
            var (totalItems, totalPages, foods) = await _foodRepository.GetSearchFood(querry, search);

            if(totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }
            var response = new
            {
                TotalItems = totalItems, // Số lượng hiện có 
                TotalPages = totalPages, // Tổng trang
                Foods = foods  // list danh sách
            };

            return Ok(response);
        }
        // https://localhost:7014/api/Food
        [HttpGet]
        public async Task<IActionResult> GetFoodAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            /* var model = await _foodRepository.GetAllFoods();
             var list = model.Select(hh => hh.ToFoodDto());
             var path = env.WebRootPath;*/
            var model = _context.Foods.Select(s => new FoodModel
            {
                FoodId = s.FoodId,
                NameFood = s.NameFood,
                UnitPrice = s.UnitPrice,
                UrlImage = s.UrlImage,
                CategoryId = s.CategoryId,
                Category = _context.Categoies.Where(a => a.CategoryId == s.CategoryId).FirstOrDefault(),

            }).ToList();


            return Ok(model);
        }
        [HttpGet]
        [Route("{foodid}")]
        public async Task<IActionResult> GetFoodById(int foodid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var food = await _context.Foods
                .Where(s => s.FoodId == foodid)
                .Select(s => new FoodModel
                {
                    FoodId = s.FoodId,
                    NameFood = s.NameFood,
                    UnitPrice = s.UnitPrice,
                    UrlImage = s.UrlImage,
                    CategoryId = s.CategoryId,
                    Category = _context.Categoies.FirstOrDefault(a => a.CategoryId == s.CategoryId)
                })
                .FirstOrDefaultAsync();

            if (food == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy thức ăn với mã ID tương ứng
            }

            return Ok(food);
        }

        // https://localhost:7014/api/Food/post-with-image
        [HttpPost("post-with-image")]
        
        public async Task<IActionResult> CreateFoodImage([FromBody]FoodImage p)
        {
            var food = new Food { CategoryId = p.CategoryId, NameFood = p.NameFood, UnitPrice = p.UnitPrice, UrlImage = p.Image };
            if(!await _categoryRespository.CategoryExit(p.CategoryId))
            {
                return BadRequest("Loại món không được tìm thấy");
            }
           /* // Xử lý ảnh
            if (p.Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    p.Image.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    food.UrlImage = base64String;
                }
            }
            else
            {
                food.UrlImage = "";
            }
*/
            _context.Foods.Add(food);
            _context.SaveChanges();

          

            return Ok(new { message = "Thành công", food });
        }

        [HttpPost]
        public async Task<ActionResult<Food>> CreateFood([FromForm] CreateFoodDTO foodDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdFood = await _foodRepository.CreateFoodAsync(foodDTO);

            return CreatedAtAction(nameof(GetFoodById), new { id = createdFood.FoodId }, createdFood);
        }
    

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateFood([FromRoute] int id, [FromBody] UpdateFoodDTO updateFood)
        {
            var model = await _foodRepository.UpdateFood(id, updateFood);
            if (model == null)
            {
                return BadRequest();
            }

            return Ok(model.ToFoodDto());
        }
     


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFood([FromRoute] int id)
        {
            var model = await _foodRepository.DeleteFood(id);
            if(model == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
