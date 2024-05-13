namespace OrderRestaurant.Service
{
    public interface IPermission
    {
        bool CheckPermission(string roleName, string function, string type);
    }
}
