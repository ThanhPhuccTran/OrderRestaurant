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
                StatusId = table.StatusId,
                Note = table.Note,
                QR_id = table.QR_id,
                
            };
        }

        public static Table ToTableFromCreate(this CreateTableDTO table)
        {
            string qrId = null;
            if (table.QR_id != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    table.QR_id.CopyTo(memoryStream);
                    qrId = Convert.ToBase64String(memoryStream.ToArray());
                }
            }

            return new Table
            {
                TableName = table.TableName,
                StatusId = 8, //set trạng thái trống
                Note = table.Note,
                QR_id = qrId
            };
        }

    }
}
