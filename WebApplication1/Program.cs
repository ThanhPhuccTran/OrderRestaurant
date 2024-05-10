
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using OrderRestaurant.Data;
using OrderRestaurant.Model;
using OrderRestaurant.Responsitory;
using OrderRestaurant.Service;
using System.Text;
namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<ICategory, CategoryResponsitory>();
            builder.Services.AddScoped<IFood, FoodResponsitory>();
            builder.Services.AddScoped<ICustomer, CustomerResponsitory>();
            builder.Services.AddScoped<IEmployee, EmployeeResponsitory>();
            builder.Services.AddScoped<ITable, TableResponsitory>();
            builder.Services.AddScoped<IOrder, OrderResponsitory>();
            builder.Services.AddScoped<IConfig, ConfigResponsitory>();
            builder.Services.AddScoped<IStatistics, StatisticsResponsitory>();

            builder.Services.AddScoped<ICommon<FoodModel>, FoodResponsitory>();
            builder.Services.AddScoped<ICommon<Table>, TableResponsitory>();
            builder.Services.AddScoped<ICommon<OrderModel>, OrderResponsitory>();

            var configuration = builder.Configuration;
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.Configure<AppSetting>(configuration.GetSection("AppSettings"));

            builder.Services.AddAuthentication(cfg =>
            {
                cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = false;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey
                        (
                            Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:SecretKey"])
                        ),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });


            // Thêm dịch vụ Session
            builder.Services.AddDistributedMemoryCache(); // Sử dụng cache bộ nhớ phân tán (cho mục đích demo)
            builder.Services.AddMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Thiết lập thời gian timeout cho session
                options.Cookie.HttpOnly = true; // Đảm bảo cookie chỉ được truy cập thông qua HTTP
                options.Cookie.IsEssential = true; // Cookie là bắt buộc để ứng dụng hoạt động đúng
            });

            //dinh nghia ra nhung cai dia chi
            /*builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));*/

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins", builder =>
                {
                    builder
                        .WithOrigins("https://localhost:7014", "http://localhost:5063")
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials()
                        .AllowAnyMethod();
                });
            });


            var app = builder.Build();
            app.UseCors("MyAllowSpecificOrigins");


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                // Middleware error handling trong môi trường production
                app.UseExceptionHandler("/Error");
                // Đảm bảo mọi truy cập HTTP được chuyển hướng sang HTTPS
                app.UseHsts();
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
                RequestPath = "/wwwroot"
            });
           
            app.UseHttpsRedirection();


            app.UseStaticFiles();

            // Thêm middleware Session vào pipeline
            app.UseSession();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();
           

            app.MapControllers();

            app.Run();
        }
    }
}
