using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using static Azure.Core.HttpHeader;

namespace OrderRestaurant.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ICategory _category;
        private readonly ICommon<CategoryModel> _common;
        public CategoryController(ApplicationDBContext context, ICategory category, ICommon<CategoryModel> common)
        {
            _context = context;
            _category = category;
            _common = common;
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


                var (totalItems, totalPages, category) = await _common.SearchAndPaginate(parameters);

                if (totalItems == 0)
                {
                    return NotFound("Không tìm thấy kết quả");
                }

                var response = new
                {
                    TotalItems = totalItems,
                    TotalPages = totalPages,
                    Categorys = category
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
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
