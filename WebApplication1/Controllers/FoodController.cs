using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.DTO.FoodDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Reponsitory;
using OrderRestaurant.Service;
using System.Security.Claims;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFood _foodRepository;
        private readonly ApplicationDBContext _context;
        private readonly ICategory _categoryRespository;
        private readonly ICommon<FoodModel> _common;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment env;
        private readonly IPermission _permissionRepository;
        private const string TYPE_FOOD = "Food";
       
        public FoodController(IFood foodRepository,ICategory categoryRespository, ApplicationDBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment env , ICommon<FoodModel> common , IPermission permissionRepository)
        {
            _foodRepository = foodRepository;
            _context = context;
            this.env = env;
            _categoryRespository = categoryRespository;
            _common = common;
            _permissionRepository = permissionRepository;
        }

        //https://localhost:7014/api/Food/search
        //Phân trang và tìm kiếm , tìm kiếm theo giá asc desc , tìm theo category
        [HttpGet("get-filter")]
        public async Task<IActionResult> Search([FromQuery] QuerryFood querry)
        {
            var filter = await _foodRepository.GetFilterFood(querry);

            if(filter == null)
            {
                return NotFound("Không tìm thấy kết quả");
            }
            
           
            return Ok(filter);
        }


        // https://localhost:7014/api/Food
        [HttpGet]
        public async Task<IActionResult> GetFoodAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                
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
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

       
        [HttpGet("get-search-page")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryObject parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
              

                var (totalItems, totalPages, foods) = await _common.SearchAndPaginate(parameters);

                if (totalItems == 0)
                {
                    return NotFound("Không tìm thấy kết quả");
                }

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Foods = foods
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("{foodid}")]
        public async Task<IActionResult> GetFoodById(int foodid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        // https://localhost:7014/api/Food/post-with-image
        [HttpPost("post-with-image")]
        
        public async Task<IActionResult> CreateFoodImage([FromBody]FoodImage p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (!_permissionRepository.CheckPermission(roleName, Constants.Post, TYPE_FOOD))
                    return Unauthorized();

                var food = new Food { CategoryId = p.CategoryId, NameFood = p.NameFood, UnitPrice = p.UnitPrice, UrlImage = p.Image };
                if (!await _categoryRespository.CategoryExit(p.CategoryId))
                {
                    return BadRequest("Loại món không được tìm thấy");
                }
                _context.Foods.Add(food);
                _context.SaveChanges();



                return Ok(new { message = "Thành công", food });
            }catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Food>> CreateFood([FromForm] CreateFoodDTO foodDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (!_permissionRepository.CheckPermission(roleName, Constants.Post, TYPE_FOOD))
                    return Unauthorized();

                var createdFood = await _foodRepository.CreateFoodAsync(foodDTO);

                return CreatedAtAction(nameof(GetFoodById), new { id = createdFood.FoodId }, createdFood);
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateFood([FromRoute] int id, [FromBody] UpdateFoodDTO updateFood)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (!_permissionRepository.CheckPermission(roleName, Constants.Put, TYPE_FOOD))
                    return Unauthorized();

                var model = await _foodRepository.UpdateFood(id, updateFood);
                if (model == null)
                {
                    return BadRequest();
                }

                return Ok(model.ToFoodDto());
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
     


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteFood([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (!_permissionRepository.CheckPermission(roleName, Constants.Delete, TYPE_FOOD))
                    return Unauthorized();

                var model = await _foodRepository.DeleteFood(id);
                if (model == null)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    }
}
