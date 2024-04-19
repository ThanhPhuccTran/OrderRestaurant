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
            string note = string.IsNullOrWhiteSpace(tableDTO.Note) ? "" : tableDTO.Note;
            tableUpdate.TableName = tableDTO.TableName;
            tableUpdate.Note = note;
            tableUpdate.StatusId = tableDTO.StatusId;
            if (tableDTO.QR_id.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    tableDTO.QR_id.CopyTo(ms);
                    var imageBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    tableUpdate.QR_id = base64String;
                }
            }
            else
            {
                tableUpdate.QR_id = "";
            }
            await _dbContext.SaveChangesAsync();
            return tableUpdate;
        }
    }
}
