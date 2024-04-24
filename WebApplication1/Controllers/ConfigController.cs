using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderRestaurant.Data;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfig _configRepository;
        private readonly ApplicationDBContext _context;
        public ConfigController(ApplicationDBContext context , IConfig configRepository)
        {
            _configRepository = configRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetConfigAll()
        {
            try
            {
                var configs = await _configRepository.GetConfig();
                if (configs == null || configs.Count == 0)
                {
                    return NotFound(); // Trả về mã lỗi 404 nếu không tìm thấy cấu hình nào
                }

                return Ok(configs); // Trả về danh sách cấu hình nếu có
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchConfig(string type = "")
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var configs = await _configRepository.SearchConfig(type);
                return Ok(configs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }


    }
}
