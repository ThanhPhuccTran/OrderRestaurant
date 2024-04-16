using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class TableResponsitory : ITable
    {
        private readonly ApplicationDBContext _dbContext;
        public TableResponsitory(ApplicationDBContext dbContext)
        {

            _dbContext = dbContext;

        }
        public async Task<Table> CreateTable(Table table)
        {
            await _dbContext.AddAsync(table);
            await _dbContext.SaveChangesAsync();
            return table;
        }

        public async Task<Table> DeleteTable(int id)
        {
            var model = await _dbContext.Tables.FirstOrDefaultAsync(i => i.TableId == id);
            if(model == null)
            {
                return null;
            }
            _dbContext.Tables.Remove(model);
            await _dbContext.SaveChangesAsync();
            return model;   
        }

        public async Task<List<Table>> GetAllTable()
        {
            return await _dbContext.Tables.ToListAsync();
        }

        public async Task<Table?> GetTableById(int tableId)
        {
            return await _dbContext.Tables.FindAsync(tableId);
        }

        public Task<bool> TableExit(int id)
        {
            return _dbContext.Tables.AnyAsync(s => s.TableId == id);
        }

        public async Task<Table> UpdateTable(int id, UpdateTableDTO tableDTO)
        {
            var tableUpdate = await _dbContext.Tables.FirstOrDefaultAsync(hh=>hh.TableId == id);
            if(tableUpdate == null)
            {
                return null;
            }
            tableUpdate.TableName = tableDTO.TableName;
            tableUpdate.Note = tableDTO.Note;
            tableUpdate.Status = tableDTO.Status;
            await _dbContext.SaveChangesAsync();
            return tableUpdate;
        }
    }
}
