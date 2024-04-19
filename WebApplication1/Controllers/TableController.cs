using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ITable _table;
        public TableController(ApplicationDBContext context , ITable table)
        {
            _context = context;
            _table = table;
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
        public async Task<IActionResult> CreateTable([FromForm] CreateTableDTO? createTable)
        {
            var model = createTable.ToTableFromCreate();
            await _table.CreateTable(model);
            return CreatedAtAction(nameof(GetTableById),new {id = model.TableId},model.ToTableDto());
        }

        [HttpPatch]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateTable([FromRoute] int id, [FromForm] UpdateTableDTO updateTableDTO)
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
