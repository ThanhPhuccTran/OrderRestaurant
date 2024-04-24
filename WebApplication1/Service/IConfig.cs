using OrderRestaurant.Data;

namespace OrderRestaurant.Service
{
    public interface IConfig
    {
        Task<List<ManageStatus>> GetConfig();

        Task<List<ManageStatus>> SearchConfig(string search = "");
    }
}
