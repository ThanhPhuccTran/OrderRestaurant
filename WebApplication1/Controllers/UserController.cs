using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OrderRestaurant.Data;
using OrderRestaurant.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly AppSetting _appSettings;
        public UserController(ApplicationDBContext context, IOptionsMonitor<AppSetting> optionsMonitor)
        {
            _context = context;
            _appSettings = optionsMonitor.CurrentValue;

        }
        [HttpPost("/Login")]
        public IActionResult Validate(LoginModel login)
        {
            var user = _context.Employees
                                    .Include(role=>role.Role)
                                    .SingleOrDefault(p => p.Email == login.Email && p.Password == login.Password);

            if (user == null)
            {
                return Unauthorized(new ApiResponse
                {
                    Success = false,
                    Message = "Unauthorized: UserName/Password không hợp lệ",
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Authenticate success",
                Data = GenerateToken(user)
            });
        }

        private TokenModel GenerateToken(Employee nhanVien)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var sercetKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,nhanVien.EmployeeName),
                    new Claim(ClaimTypes.Email , nhanVien.Email),
                    new Claim("Phone",nhanVien.Phone),
                    new Claim("EmployeeId",nhanVien.EmployeeId.ToString()),
                   new Claim(ClaimTypes.Role, nhanVien.Role.RoleName),
                    //roles


                    new Claim("TokenId",Guid.NewGuid().ToString())
                }),
                //Time hết hạn 
                Expires = DateTime.UtcNow.AddMinutes(60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(sercetKeyBytes), SecurityAlgorithms.HmacSha512Signature)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);

            var accessToken = jwtTokenHandler.WriteToken(token);

            return new TokenModel { AccessToken = accessToken, RefreshToken = GenerateRefreshToken() };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random);
        }
    }
}
