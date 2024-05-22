using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;

namespace OrderRestaurant.Service
{
    public interface ITable
    {
        Task<(int totalItems, int totalPages, List<Table> tables)> GetSearch(QuerryObject querry, string search = "");
        Task<List<Table>> GetAllTable();
        Task<Table?> GetTableById(int tableId);
        Task<Table> CreateTable(Table table);
        Task<Table> UpdateTable(int id,UpdateTableDTO tableDTO);
        Task<Table> DeleteTable(int id);
        Task<bool> TableExit(int id);
        Task<Table> PostBooking(int  id);
        Task<Table> CancelBooking(int id);
        Task<Table> CheckInBooking(int id);
    }
}
