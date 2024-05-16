using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.CategoryDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using System.Security.Claims;
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
        private readonly IPermission _permissionRepository;
        private const string TYPE_Category = "Category";
        public CategoryController(ApplicationDBContext context, ICategory category, ICommon<CategoryModel> common, IPermission permissionRepository)
        {
            _context = context;
            _category = category;
            _common = common;
            _permissionRepository = permissionRepository;
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

                if (!_permissionRepository.CheckPermission(roleName, Constants.Put, TYPE_Category))
                    return Unauthorized();
                var model = await _category.UpdateCategory(id, updateCategory);
                if (model == null)
                {
                    return BadRequest();
                }

                return Ok(model.ToCategoryDto());
            }
            catch (Exception ex) {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDTO category)
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

                if (!_permissionRepository.CheckPermission(roleName, Constants.Post, TYPE_Category))
                    return Unauthorized();
                var model = category.ToCategoryFromCreate();
                await _category.CreateCategory(model);
                return CreatedAtAction(nameof(GetCategoryById), new { id = model.CategoryId }, model.ToCategoryDto());
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{categoryid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute]int categoryid)
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

                if (!_permissionRepository.CheckPermission(roleName, Constants.Delete, TYPE_Category))
                    return Unauthorized();
                var model = await _category.DeleteCategory(categoryid);
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
