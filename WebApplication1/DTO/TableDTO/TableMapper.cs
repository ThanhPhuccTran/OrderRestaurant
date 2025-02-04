﻿using OrderRestaurant.Data;
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
                Code = table.Code,
                Note = table.Note,
                QR_id = table.QR_id,
                
            };
        }

        public static Table ToTableFromCreate(this CreateTableDTO table)
        {
            if (table.QR_id == null || table.QR_id.Length == 0)
            {
                return null;
            }

            /*string qrId;
            using (var memoryStream = new MemoryStream())
            {
                table.QR_id.CopyTo(memoryStream);
                qrId = Convert.ToBase64String(memoryStream.ToArray());
            }*/

            string note = string.IsNullOrWhiteSpace(table.Note) ? "" : table.Note;

            return new Table
            {
                TableName = table.TableName,
                Code = Constants.TABLE_EMPTY, // Set trạng thái trống
                Note = note,
                QR_id = table.QR_id
            };
        }


    }
}
