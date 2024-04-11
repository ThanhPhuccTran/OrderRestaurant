using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.DTO.FoodDTO;
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
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment env;
        public FoodController(IFood foodRepository, ApplicationDBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            _foodRepository = foodRepository;
            _context = context;
            this.env = env;
        }
        [HttpGet]
        public async Task<IActionResult> GetFoodAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _foodRepository.GetAllFoods();
            var list = model.Select(hh => hh.ToFoodDto());
            var path = env.WebRootPath;
            return Ok(new {list = list , path = path});
        }

        [HttpPost("post-with-image")]
        
        public async Task<IActionResult> CreateFoodImage([FromForm]FoodImage p)
        {
            var food = new Food { CategoryId = p.CategoryId, NameFood = p.NameFood, UnitPrice = p.UnitPrice };

            // Xử lý ảnh
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

            _context.Foods.Add(food);
            _context.SaveChanges();

            var baseUrl = $"{this.Request.Scheme}:{this.Request.Host}";
            var imageUrl = $"{baseUrl}/{food.UrlImage}";

            return Ok(new { message = "Thành công", food = food, imageUrl = imageUrl });
        }
        [HttpPost]
        public async Task<ActionResult<Food>> CreateFood([FromForm] CreateFoodDTO foodDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdFood = await _foodRepository.CreateFoodAsync(foodDTO);

            return CreatedAtAction(nameof(GetFood), new { id = createdFood.FoodId }, createdFood);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Food>> GetFood(int id)
        {
            var food = await _foodRepository.GetFoodByIdAsync(id);

            if (food == null)
            {
                return NotFound();
            }

            return food;
        }
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateFood([FromRoute] int id, [FromForm] UpdateFoodDTO updateFood)
        {
            var model = await _foodRepository.UpdateFood(id, updateFood);
            if (model == null)
            {
                return BadRequest();
            }

            return Ok(model.ToFoodDto());
        }


    }
}
