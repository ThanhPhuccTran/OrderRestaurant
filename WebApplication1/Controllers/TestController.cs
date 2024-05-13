using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OrderRestaurant.Controllers
{
    [Authorize] // Đảm bảo chỉ người dùng đã đăng nhập mới có thể truy cập endpoint này
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("test-role")]
        public IActionResult TestRole()
        {
            // Lấy dữ liệu JWT từ request
            var jwt = HttpContext.GetTokenAsync("access_token").Result;
            if (jwt != null)
            {
                // In dữ liệu JWT để kiểm tra
                Console.WriteLine("JWT data sssssssss : " + jwt);
            }

            // In ra danh sách các claim từ User.Claims
            Console.WriteLine("Claims     ssss :");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type}: {claim.Value}");
            }

            // Kiểm tra xem claim "role" có trong danh sách các claim hay không
            var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            Console.WriteLine($"{roleName   }");
            if (roleName != null)
            {
                return Ok($"Vai trò của người dùng: {roleName}");
            }
            else
            {
                return BadRequest("Không tìm thấy thông tin về vai trò của người dùng.");
            }
        }

    }
}
