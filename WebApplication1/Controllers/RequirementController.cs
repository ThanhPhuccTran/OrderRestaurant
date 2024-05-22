using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.RequirementDTO;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Responsitory;
using OrderRestaurant.Service;

namespace OrderRestaurant.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequirementController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IRequest _request;
        private readonly ICommon<RequirementModel> _common;
        public RequirementController(ApplicationDBContext context,IRequest request, ICommon<RequirementModel> common)
        {
            _context = context;
            _request = request;
            _common = common;
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
                var model = await _request.GetAllRequestsAsync();
                if(model == null)
                {
                    return NotFound("Không tìm thấy");
                }

                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }
        [HttpGet("get-search")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryOrder parameters)
        {
            var (totalItems, totalPages, request) = await _request.SearchAndPaginate(parameters);

            if (totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Request = request
            };

            return Ok(response);
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
                var model = await _request.GetRequestByIdAsync(id);
            if(model == null)
            {
                return NotFound("Không tìm thấy");
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
                var model = await _request.CreateRequestAsync(requirement);
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
                var request = await _request.CompleteRequestAsync(requestId);
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
               var model = await _request.RefuseRequestAsync(requestId);
                if(!model)
                {
                    return NotFound("Không tìm thấy");
                }

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
                var model = await _request.DeleteRequestAsync(id);
                if (!model)
                {
                    return NotFound("Không tìm thấy yêu cầu");
                }
               
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
                var request = await _request.UpdateRequestAsync(requestId,updatedRequirement);
                if (!request)
                {
                    return NotFound("Không tìm thấy yêu cầu");
                }

                return Ok("Thông tin yêu cầu đã được cập nhật thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Bị lỗi: {ex.Message}");
            }
        }


        [HttpGet("get-search-page")]
        public async Task<IActionResult> SearchAndPaginate([FromQuery] QuerryObject parameters)
        {
            var (totalItems, totalPages, request) = await _common.SearchAndPaginate(parameters);

            if (totalItems == 0)
            {
                return NotFound("Không tìm thấy kết quả");
            }

            var response = new
            {
                TotalItems = totalItems,
                TotalPages = totalPages,
                Request = request
            };

            return Ok(response);
        }



    }
}
