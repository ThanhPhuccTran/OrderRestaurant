using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class ConfigReponsitory : IConfig
    {
        private readonly ApplicationDBContext _context;
        public ConfigReponsitory(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<ManageStatus>> GetConfig()
        {
            return await _context.Statuss.ToListAsync();
        }

        public async Task<List<ManageStatus>> SearchConfig(string Type = "")
        {
            var querry = _context.Statuss.AsQueryable();
            if(!string.IsNullOrEmpty(Type))
            {
                querry = querry.Where(s => s.Type.Contains(Type));
            }
            return await querry.ToListAsync();
        }
    }
}
