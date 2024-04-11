
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using OrderRestaurant.Data;
using OrderRestaurant.Responsitory;
using OrderRestaurant.Service;
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

            builder.Services.AddDbContext<ApplicationDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            //dinh nghia ra nhung cai dia chi
            builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
           
            var app = builder.Build();

            

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
                RequestPath = "/wwwroot"
            });
            app.UseHttpsRedirection();
        


            app.UseAuthorization();
           

            app.MapControllers();

            app.Run();
        }
    }
}
