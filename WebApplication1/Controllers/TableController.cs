using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ITable _table;
        private readonly ICommon<Table> _common;
        public TableController(ApplicationDBContext context , ITable table , ICommon<Table> common)
        {
            _context = context;
            _table = table;
            _common = common;
        }

        

        [HttpGet("get-search-page")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryObject parameters)
        {
            var (totalItems, totalPages, tables) = await _common.SearchAndPaginate(parameters);

            if (totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Tabkes = tables
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetTableAll()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _table.GetAllTable();
            var list = model.Select(hh => hh.ToTableDto());
            return Ok(list);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTableById([FromRoute] int id)
        {
            bool check = await _table.TableExit(id);
            if (!check)
            {
                return NotFound("Không tìm thấy bàn");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var model = await _table.GetTableById(id);
            if(model == null)
            {
                return BadRequest(ModelState);  
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTable([FromBody] CreateTableDTO? createTable)
        {
            var model = createTable.ToTableFromCreate();
            if(model == null)
            {
                return BadRequest("Bắt buộc bạn phải nhập ảnh");
            }
            await _table.CreateTable(model);
            return CreatedAtAction(nameof(GetTableById),new {id = model.TableId},model.ToTableDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateTable([FromRoute] int id, [FromBody] UpdateTableDTO updateTableDTO)
        {
            bool check = await _table.TableExit(id);
            if (!check)
            {
                return NotFound("Không tìm thấy bàn");
            }

            // Kiểm tra xem thuộc tính Name được gửi và không rỗng
            if (updateTableDTO.TableName != null && updateTableDTO.TableName.Trim() == "")
            {
                return BadRequest("Tên không được để trống");
            }
            if(updateTableDTO.QR_id == null)
            {
                return BadRequest("Mã QR không được để trống");
            }

            var model = await _table.UpdateTable(id, updateTableDTO);

            if (model == null)
            {
                return BadRequest(ModelState);
            }

            return Ok(model.ToTableDto());
        }

        [HttpDelete]
        [Route("{tableid}")]
        public async Task<IActionResult> DeleteTable([FromRoute] int tableid)
        {
            bool check = await _table.TableExit(tableid);
            if (!check)
            {
                return NotFound("Không tìm thấy bàn");
            }
            var model = await _table.DeleteTable(tableid);
            if(model == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
