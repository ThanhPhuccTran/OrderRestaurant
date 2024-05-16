using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.RequirementDTO;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Model;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public RequirementController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet("get-request-all")]
        public async Task<IActionResult> GetAllRequest()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _context.Requests
                    .OrderByDescending(s => s.RequestTime)
                    .Select(s => new RequirementModel
                    {
                        RequestId = s.RequestId,
                        TableId = s.TableId,
                        RequestTime = s.RequestTime,
                        Title = s.Title,
                        RequestNote = s.RequestNote,
                        Code = s.Code,
                        Tables = _context.Tables.Where(a => a.TableId == s.TableId)
                            .Select(o => new TablesDTO
                            {
                                TableId = o.TableId,
                                TableName = o.TableName,
                                Note = o.Note,
                                QR_id = o.QR_id,
                                Code = o.Code
                            })
                            .FirstOrDefault()??new TablesDTO(),
                        Statuss = _context.Statuss
                            .Where(a => a.Code == s.Code && a.Type == "Order")
                            .Select(o => new ManageStatusDTO
                            {
                                StatusId = o.StatusId,
                                Code = o.Code,
                                Type = o.Type,
                                Value = o.Value,
                                Description = o.Description,
                            })
                            .FirstOrDefault(),
                    }).ToListAsync();

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _context.Requests.Select
                    (
                        s => new RequirementModel
                        {
                            RequestId = s.RequestId,
                            TableId = s.TableId,
                            RequestTime = s.RequestTime,
                            Title = s.Title,
                            RequestNote = s.RequestNote,
                            Code = s.Code,
                            Tables = _context.Tables.Where(a => a.TableId == s.TableId)
                                                    .Select(o => new TablesDTO
                                                    {
                                                        TableId = o.TableId,
                                                        TableName = o.TableName,
                                                        Note = o.Note,
                                                        QR_id = o.QR_id,
                                                        Code = o.Code
                                                    }).FirstOrDefault() ?? new TablesDTO(),
                            Statuss = _context.Statuss
                                                .Where(a => a.Code == s.Code && a.Type == "Order")
                                                .Select(o => new ManageStatusDTO
                                                {
                                                    StatusId = o.StatusId,
                                                    Code = o.Code,
                                                    Type = o.Type,
                                                    Value = o.Value,
                                                    Description = o.Description,
                                                }).FirstOrDefault(),

                         }).Where(a=>a.RequestId == id).FirstOrDefaultAsync();
            if(model == null)
            {
                return NotFound();
            }
                return Ok(model);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }

        [HttpPost("request")]
        public async Task<IActionResult> TakeRequest ([FromBody]CreatedRequirementDTO requirement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var table = await _context.Tables.FindAsync(requirement.TableId);
                if (table == null)
                {
                    return NotFound("Không tìm thấy bàn");
                }
                var model = new Requirements
                {
                    TableId = requirement.TableId,
                    RequestTime = DateTime.Now,
                    RequestNote = requirement.RequestNote,
                    Title = requirement.Title,
                    Code = Constants.REQUEST_INIT, 
                };
                //Notifi
                var notifi = new Notification
                {
                    Title = "Có yêu cầu mới",
                    Content = "",
                    Type = "Requirements",
                    IsCheck = false,
                    CreatedAt = DateTime.Now,
                };
                _context.Notifications.Add(notifi);
                _context.Requests.Add(model);
                await _context.SaveChangesAsync();
                return Ok("Yêu cầu thành công");
            }catch(Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpPost("complete-request/{requestId}")]
        public async Task<IActionResult> CompleteRequest(int requestId)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    return NotFound("Không tìm thấy yêu cầu");
                }
                if (request.Code != Constants.REQUEST_INIT)
                {
                    return BadRequest("Trạng thái không phải là yêu cầu mới");
                }

                request.Code = Constants.REQUEST_COMPLETE; 
                var requestDTO = new RequestDTO 
                { 
                    RequestId = request.RequestId,
                    TableId = request.TableId,
                    RequestTime = request.RequestTime,
                    Code = request.Code,
                    Title = request.Title,
                    RequestNote = request.RequestNote,
                    Tables = _context.Tables.Where(a => a.TableId == request.TableId)
                            .Select(o => new TablesDTO
                            {
                                TableId = o.TableId,
                                TableName = o.TableName,
                                Note = o.Note,
                                QR_id = o.QR_id,
                                Code = o.Code
                            })
                            .FirstOrDefault() ?? new TablesDTO(),

                };
                _context.Requests.Update(request);
                await _context.SaveChangesAsync();
                return Ok("Yêu cầu đã hoàn thành");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }

        [HttpPost("refuse-request/{requestId}")]
        public async Task<IActionResult> RefuseRequest(int requestId)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    return NotFound("Không tìm thấy yêu cầu");
                }
                if (request.Code != Constants.REQUEST_INIT)
                {
                    return BadRequest("Trạng thái không phải là yêu cầu mới");
                }

                request.Code = Constants.REQUEST_REFUSE;
                var requestDTO = new RequestDTO
                {
                    RequestId = request.RequestId,
                    TableId = request.TableId,
                    RequestTime = request.RequestTime,
                    Code = request.Code,
                    Title = request.Title,
                    RequestNote = request.RequestNote,
                    Tables = _context.Tables.Where(a => a.TableId == request.TableId)
                            .Select(o => new TablesDTO
                            {
                                TableId = o.TableId,
                                TableName = o.TableName,
                                Note = o.Note,
                                QR_id = o.QR_id,
                                Code = o.Code
                            })
                            .FirstOrDefault() ?? new TablesDTO(),

                };
                await _context.SaveChangesAsync();

                return Ok("Yêu cầu đã bị từ chối");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var model = await _context.Requests.FindAsync(id);
                if (model == null)
                {
                    return NotFound("Không tìm thấy yêu cầu");
                }
                _context.Requests.Remove(model);
                await _context.SaveChangesAsync();
                return Ok("Xóa thành công");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }

        [HttpPut("update-request/{requestId}")]
        public async Task<IActionResult> UpdateRequest(int requestId, [FromBody] UpdatedRequirementDTO updatedRequirement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    return NotFound("Không tìm thấy yêu cầu");
                }

                // Kiểm tra xem yêu cầu có trong trạng thái cho phép sửa hay không
                if (request.Code != Constants.REQUEST_INIT)
                {
                    return BadRequest("Không thể sửa yêu cầu không phải là yêu cầu mới");
                }

                // Cập nhật thông tin yêu cầu
                request.Title = updatedRequirement.Title;
                request.RequestNote = updatedRequirement.RequestNote;

                // Lưu các thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                return Ok("Thông tin yêu cầu đã được cập nhật thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }


        [HttpGet("search-request")]
        public async Task<IActionResult> SearchRequest(string search="", int page = 1, int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var requests = await _context.Requests
                    .Where(r => EF.Functions.Like(r.Title, $"%{search}%"))
                    .OrderByDescending(s => s.RequestTime)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new RequirementModel
                    {
                        RequestId = s.RequestId,
                        TableId = s.TableId,
                        RequestTime = s.RequestTime,
                        Title = s.Title,
                        RequestNote = s.RequestNote,
                        Code = s.Code,
                        Tables = _context.Tables.Where(a => a.TableId == s.TableId)
                            .Select(o => new TablesDTO
                            {
                                TableId = o.TableId,
                                TableName = o.TableName,
                                Note = o.Note,
                                QR_id = o.QR_id,
                                Code = o.Code
                            })
                            .FirstOrDefault() ?? new TablesDTO(),
                        Statuss = _context.Statuss
                            .Where(a => a.Code == s.Code && a.Type == "Order")
                            .Select(o => new ManageStatusDTO
                            {
                                StatusId = o.StatusId,
                                Code = o.Code,
                                Type = o.Type,
                                Value = o.Value,
                                Description = o.Description,
                            })
                            .FirstOrDefault(),
                    }).ToListAsync();

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }



    }
}
