using OrderRestaurant.Helpers;

namespace OrderRestaurant.Service
{
    public interface ICommon<T> where T : class
    {
        Task<(int totalItems, int totalPages, List<T> items)> SearchAndPaginate(QuerryObject querryObject);
        
    }
}
