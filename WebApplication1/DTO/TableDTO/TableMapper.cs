using OrderRestaurant.Data;
using OrderRestaurant.Model;

namespace OrderRestaurant.DTO.TableDTO
{
    public static class TableMapper
    {
        public static TableModel ToTableDto (this Table table)
        {
            return new TableModel
            {
                TableId = table.TableId,
                TableName = table.TableName,
                Status = table.Status,
                Note = table.Note,
                
            };
        }

        public static Table ToTableFromCreate(this CreateTableDTO table)
        {
            return new Table
            {
                TableName = table.TableName,
                Status = 8, //set trạng thái trống
                Note = table.Note,
            };
        }
    }
}
