using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
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
        public async Task<IActionResult> Validate(LoginModel login)
        {
            try
            {
                var user = _context.Employees
                                        .SingleOrDefault(p => p.Email == login.Email && p.Password == login.Password);

                if (user == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        Success = false,
                        Message = "Unauthorized: UserName/Password không hợp lệ",
                    });
                }
                //cấp token 
                var token = await GenerateToken(user);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Authenticate success",
                    Data = token,
                });
            }catch(Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = true,
                    Message = "Authenticate failed",
                    Data = null,
                });
            }
        }

        private async Task<TokenModel> GenerateToken(Employee nhanVien)
        {

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var sercetKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
           
           
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name,nhanVien.EmployeeName),
                    new Claim(JwtRegisteredClaimNames.Email , nhanVien.Email),
                    new Claim(JwtRegisteredClaimNames.Sub , nhanVien.Email),
                    new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()), //lưu vào JWTId
                    new Claim("EmployeeId",nhanVien.EmployeeId.ToString()),
                    new Claim(ClaimTypes.Role, nhanVien.RoleName),
                   
                }),
                //Time hết hạn 
                Expires = DateTime.UtcNow.AddSeconds(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(sercetKeyBytes), SecurityAlgorithms.HmacSha512Signature)

            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);

            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();
            //Lưu database
            var refreshTokenEntity = new RefreshToken
            {
                RefreshTokeId = Guid.NewGuid(),
                JwtId = token.Id,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssueAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddSeconds(15),
                EmployeeId = nhanVien.EmployeeId,

            };

            await _context.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();


            return new TokenModel 
            { 
                AccessToken = accessToken, 
                RefreshToken = refreshToken,
            };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[128];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random);
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var sercetKeyBytes = Encoding.UTF8.GetBytes(_appSettings.SecretKey);
            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(sercetKeyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false, // không kiểm tra về thời gian hết hạn
            };
            try
            {
                // Kiểm tra 1: AccessToken có định dạng hợp lệ không
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

                // Kiểm tra alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Token không hợp lệ",
                        });
                    }
                }

                // Kiểm tra 2: Xác định thời gian hết hạn từ AccessToken
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Access token vẫn còn hiệu lực",
                    });
                }


                // Kiểm tra 3: Kiểm tra RefreshToken có tồn tại trong cơ sở dữ liệu không
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == model.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token không tồn tại",
                    });
                }
                // Kiểm tra thời gian hết hạn của RefreshToken
                if (storedToken.ExpiredAt < DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token đã hết hạn",
                    });
                }
                // Kiểm tra 4: Kiểm tra RefreshToken đã được sử dụng hoặc bị thu hồi chưa
                if (storedToken.IsUsed)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token đã được sử dụng",
                    });
                }

                if (storedToken.IsRevoked)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token đã bị thu hồi",
                    });
                }

                // Kiểm tra 5: So sánh AccessToken Id với JwtId trong RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token không khớp",
                    });
                }

                // Cập nhật trạng thái của RefreshToken
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();

                // Tạo mới AccessToken và trả về
                var user = await _context.Employees.SingleOrDefaultAsync(nd => nd.EmployeeId == storedToken.EmployeeId);
                var token = await GenerateToken(user);
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Tạo mới token thành công",
                    Data = token,
                });

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Có lỗi xảy ra",
                });
            }
        }


        private DateTime ConvertUnixTimeToDateTime(long utcExpriceDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpriceDate).ToUniversalTime();
            return dateTimeInterval;
        }
    }
}
