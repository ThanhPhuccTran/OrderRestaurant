using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.Service;

namespace OrderRestaurant.Reponsitory
{
    public class PermissionReponsitory : IPermission
    {
        private readonly ApplicationDBContext _context;
        public PermissionReponsitory(ApplicationDBContext context)
        {
            _context = context;
        }
        public bool CheckPermission(string roleName, string function, string type)
        {
            if (roleName == "admin")
            {
                return true; 
            }
            var permission = _context.Permissions.FirstOrDefault(p => p.RoleName == roleName && p.Function == function && p.FunctionTable == type);
            return permission != null;
        }
    }
}
