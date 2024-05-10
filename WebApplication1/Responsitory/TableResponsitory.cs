using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Responsitory
{
    public class TableResponsitory : ITable,ICommon<Table>
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

        public async Task<(int totalItems, int totalPages, List<Table> tables)> GetSearch(QuerryObject querry, string search = "")
        {
            var list = _dbContext.Tables.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                list = list.Where(f => EF.Functions.Like(f.TableName, $"%{search}%"));

            }
            var totalItems = await list.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / querry.PageSize);
            var skipNumber = (querry.PageNumber - 1) * querry.PageSize;

            var table = await list.Select(f => new Table
            {
                TableId = f.TableId,
                TableName = f.TableName,
                Note = f.Note,
                QR_id = f.QR_id,
                Code = f.Code,
               /* Statuss = f.Statuss*/
            }).Skip(skipNumber)
                .Take(querry.PageSize)
                .ToListAsync();
            return (totalItems, totalPages, table);
        }

        public async Task<Table?> GetTableById(int tableId)
        {
            return await _dbContext.Tables.FindAsync(tableId);
        }

        public async Task<(int totalItems, int totalPages, List<Table> items)> SearchAndPaginate(QuerryObject querryObject)
        {
            var query = _dbContext.Tables.AsQueryable();

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrWhiteSpace(querryObject.Search))
            {
                query = query.Where(f => EF.Functions.Like(f.TableName, $"%{querryObject.Search}%"));
            }

            // Tính toán số trang và lấy dữ liệu phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / querryObject.PageSize);
            var skipNumber = (querryObject.PageNumber - 1) * querryObject.PageSize;
            var items = await query.Skip(skipNumber).Take(querryObject.PageSize).ToListAsync();

            return (totalItems, totalPages, items);
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
            tableUpdate.Code = Constants.TABLE_EMPTY;
            tableUpdate.QR_id = tableDTO.QR_id;
           /* if (tableDTO.QR_id.Length > 0)
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
            }*/
            await _dbContext.SaveChangesAsync();
            return tableUpdate;
        }
    }
}
