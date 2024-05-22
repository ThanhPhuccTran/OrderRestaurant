
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using OrderRestaurant.Data;
using OrderRestaurant.Hubs;
using OrderRestaurant.Model;
using OrderRestaurant.Reponsitory;
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

            builder.Services.AddScoped<ICategory, CategoryReponsitory>();
            builder.Services.AddScoped<IFood, FoodReponsitory>();
            builder.Services.AddScoped<ICustomer, CustomerReponsitory>();
            builder.Services.AddScoped<IEmployee, EmployeeReponsitory>();
            builder.Services.AddScoped<ITable, TableReponsitory>();
            builder.Services.AddScoped<IOrder, OrderReponsitory>();
            builder.Services.AddScoped<IConfig, ConfigReponsitory>();
            builder.Services.AddScoped<IStatistics, StatisticsReponsitory>();
            builder.Services.AddScoped<IPermission, PermissionReponsitory>();
            builder.Services.AddScoped<INotification, NotificationReponsitory>();
            builder.Services.AddScoped<IRequest, RequestReponsitory>();

            /*builder.Services.AddScoped<ICommon<FoodModel>, FoodReponsitory>();*/
            builder.Services.AddScoped<ICommon<Table>, TableReponsitory>();
            builder.Services.AddScoped<ICommon<OrderModel>, OrderReponsitory>();
            builder.Services.AddScoped<ICommon<CategoryModel>, CategoryReponsitory>();
            builder.Services.AddScoped<ICommon<EmployeeModel>, EmployeeReponsitory>();
            builder.Services.AddScoped<ICommon<RequirementModel>, RequestReponsitory>();

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
                /*x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken)
                            && path.StartsWithSegments("/notificationHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;

                    }
                };*/

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
            /* builder.Services.AddSignalR();*/
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
            /*  app.MapHub<NotificationHub>("/notificationHub");*/
            /*  app.UseEndpoints(endpoints =>
              {
                  endpoints.MapControllers();
                  endpoints.MapHub<NotificationHub>("/notificationHub");
              });*/

            app.Run();
        }
    }
}
