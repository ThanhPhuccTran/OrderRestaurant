using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.TableDTO;

namespace OrderRestaurant.Service
{
    public interface ITable
    {
        Task<List<Table>> GetAllTable();
        Task<Table?> GetTableById(int tableId);
        Task<Table> CreateTable(Table table);
        Task<Table> UpdateTable(int id,UpdateTableDTO tableDTO);
        Task<Table> DeleteTable(int id);
        Task<bool> TableExit(int id);
    }
}
