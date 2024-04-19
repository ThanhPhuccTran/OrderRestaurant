using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ICategory _category;
        public CategoryController(ApplicationDBContext context, ICategory category)
        {
            _context = context;
            _category = category;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] QuerryObject querry, string search = "")
        {
            if (querry.PageNumber <= 0 || querry.PageSize <= 0)
            {
                return BadRequest("Không hợp lệ");
            }
            var (totalItems, totalPages, category) = await _category.GetSearch(querry, search);
            if (totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }
            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Category = category
            };
            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetCategoryAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _category.GetCategoryFoods();
            var list = model.Select(hh => hh.ToCategoryDto());
            return Ok(list);
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _category.GetCategoryFoodById(id);
            if (model == null)
            {
                return BadRequest(ModelState);
            }
            return Ok(model);
        }
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryDTO updateCategory)
        {
            var model = await _category.UpdateCategory(id, updateCategory);
            if (model == null)
            {
                return BadRequest();
            }

            return Ok(model.ToCategoryDto());

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO category)
        {
            var model = category.ToCategoryFromCreate();
            await _category.CreateCategory(model);
            return CreatedAtAction(nameof(GetCategoryById), new { id = model.CategoryId }, model.ToCategoryDto());
        }

        [HttpDelete]
        [Route("{categoryid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute]int categoryid)
        {
            var model = await _category.DeleteCategory(categoryid);
            if (model == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
