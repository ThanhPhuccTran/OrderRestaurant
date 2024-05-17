using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;
using System.Security.Claims;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ITable _table;
        private readonly ICommon<Table> _common;
        private readonly IPermission _permissionRepository;
        private const string TYPE_Table = "Table";
        public TableController(ApplicationDBContext context , ITable table , ICommon<Table> common, IPermission permissionRepository)
        {
            _context = context;
            _table = table;
            _common = common;
            _permissionRepository = permissionRepository;
        }

        

        [HttpGet("employee/get-search-page")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryObject parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }
                if (!_permissionRepository.CheckPermission(roleName, Constants.Get, TYPE_Table))
                    return Unauthorized();
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
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
        [HttpGet("admin/get-search-page")]
        public async Task<IActionResult> SearchAndPaginateAdmin([FromQuery] QuerryObject parameters)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (roleName != "admin")
                {
                    return Unauthorized("Chỉ admin mới có quyền truy cập vào phương thức này.");
                }
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
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("employee/get-table")]
        public async Task<IActionResult> GetTableAll()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }
                if (!_permissionRepository.CheckPermission(roleName, Constants.Get, TYPE_Table))
                    return Unauthorized();

                var model = await _table.GetAllTable();
                var list = model.Select(hh => hh.ToTableDto());
                return Ok(list);
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpGet("admin/get-table")]
        public async Task<IActionResult> GetAdminTableAll()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (roleName != "admin")
                {
                    return Unauthorized("Chỉ admin mới có quyền truy cập vào phương thức này.");
                }

                var model = await _table.GetAllTable();
                var list = model.Select(hh => hh.ToTableDto());
                return Ok(list);

            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (roleName != "admin")
                {
                    return Unauthorized("Chỉ admin mới có quyền truy cập vào phương thức này.");
                }
                var model = createTable.ToTableFromCreate();
                if (model == null)
                {
                    return BadRequest("Bắt buộc bạn phải nhập ảnh");
                }
                await _table.CreateTable(model);
                return CreatedAtAction(nameof(GetTableById), new { id = model.TableId }, model.ToTableDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateTable([FromRoute] int id, [FromBody] UpdateTableDTO updateTableDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (roleName != "admin")
                {
                    return Unauthorized("Chỉ admin mới có quyền truy cập vào phương thức này.");
                }
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
                if (updateTableDTO.QR_id == null)
                {
                    return BadRequest("Mã QR không được để trống");
                }

                var model = await _table.UpdateTable(id, updateTableDTO);

                if (model == null)
                {
                    return BadRequest(ModelState);
                }

                return Ok(model.ToTableDto());
            }catch(Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{tableid}")]
        public async Task<IActionResult> DeleteTable([FromRoute] int tableid)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                if (roleName == null)
                {
                    return BadRequest("Ko co rolename");
                }

                if (roleName != "admin")
                {
                    return Unauthorized("Chỉ admin mới có quyền truy cập vào phương thức này.");
                }
                bool check = await _table.TableExit(tableid);
                if (!check)
                {
                    return NotFound("Không tìm thấy bàn");
                }
                var model = await _table.DeleteTable(tableid);
                if (model == null)
                {
                    return NotFound("Không tìm thấy bàn");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi: {ex.Message}");
            }
        }
    }
}
