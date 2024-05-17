using Azure.Core;
using Microsoft.EntityFrameworkCore;
using OrderRestaurant.Data;
using OrderRestaurant.DTO.ConfigDTO;
using OrderRestaurant.DTO.EmployeeDTO;
using OrderRestaurant.DTO.RequirementDTO;
using OrderRestaurant.DTO.TableDTO;
using OrderRestaurant.Helpers;
using OrderRestaurant.Model;
using OrderRestaurant.Service;

namespace OrderRestaurant.Reponsitory
{
    public class RequestReponsitory : IRequest , ICommon<RequirementModel>
    {
        private readonly ApplicationDBContext _context;
        public RequestReponsitory(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> CompleteRequestAsync(int requestId)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    throw new Exception("Yêu cầu không tìm thấy");
                }

                if (request.Code != Constants.REQUEST_INIT)
                {
                    throw new Exception("Yêu cầu không phải là mới nhất");
                }

                request.Code = Constants.REQUEST_COMPLETE;
                _context.Requests.Update(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        public async Task<int> CreateRequestAsync(CreatedRequirementDTO requirement)
        {
            try
            {
                var table = await _context.Tables.FindAsync(requirement.TableId);
                if (table == null)
                {
                    throw new Exception("Không tìm thấy bàn");
                }

                var model = new Requirements
                {
                    TableId = requirement.TableId,
                    RequestTime = DateTime.Now,
                    RequestNote = requirement.RequestNote,
                    Title = requirement.Title,
                    Code = Constants.REQUEST_INIT,
                };

                // Notification
                var notifi = new Notification
                {
                    Title = "Có yêu cầu mới",
                    Content = $"{model.Title}",
                    Type = Constants.TYPE_REQUEST,
                    IsCheck = false,
                    CreatedAt = DateTime.Now,
                };

                _context.Notifications.Add(notifi);
                _context.Requests.Add(model);
                await _context.SaveChangesAsync();
                return model.RequestId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> DeleteRequestAsync(int id)
        {
            try
            {
                var request = await _context.Requests.FindAsync(id);
                if (request == null)
                {
                    throw new Exception("Yêu cầu không được tìm thấy");
                }

                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        public async Task<List<RequirementModel>> GetAllRequestsAsync()
        {
            try
            {
                var requests = await _context.Requests
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
                            .FirstOrDefault() ?? new TablesDTO(),
                        Statuss = _context.Statuss
                            .Where(a => a.Code == s.Code && a.Type == Constants.TYPE_REQUEST)
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

                return requests;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        public async Task<RequirementModel> GetRequestByIdAsync(int id)
        {
            try
            {
                var request = await _context.Requests
                    .Where(a => a.RequestId == id)
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
                            .Where(a => a.Code == s.Code && a.Type == Constants.TYPE_REQUEST)
                            .Select(o => new ManageStatusDTO
                            {
                                StatusId = o.StatusId,
                                Code = o.Code,
                                Type = o.Type,
                                Value = o.Value,
                                Description = o.Description,
                            })
                            .FirstOrDefault(),
                    }).FirstOrDefaultAsync();

                return request;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        public async Task<bool> RefuseRequestAsync(int requestId)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    throw new Exception("Yêu cầu không được tìm thấy");
                }

                if (request.Code != Constants.REQUEST_INIT)
                {
                    throw new Exception("Trạng thái yêu cầu không phải là mới nhất");
                }

                request.Code = Constants.REQUEST_REFUSE;
                _context.Requests.Update(request);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }

        public async Task<(int totalItems, int totalPages, List<RequirementModel> items)> SearchAndPaginate(QuerryObject querryObject)
        {
            var query = _context.Requests
                                  .OrderByDescending(s => s.RequestTime)
                                  .Include(o => o.Tables)
                                  .AsQueryable();

            // Áp dụng tìm kiếm nếu có
            if (!string.IsNullOrWhiteSpace(querryObject.Search))
            {
                query = query.Where(f => EF.Functions.Like(f.Tables.TableName, $"%{querryObject.Search}%"));
            }

            // Tính toán số trang và lấy dữ liệu phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / querryObject.PageSize);
            var skipNumber = (querryObject.PageNumber - 1) * querryObject.PageSize;
            var items = await query.Skip(skipNumber).Take(querryObject.PageSize).ToListAsync();
            var requestModel = items.Select(s => new RequirementModel
            {
                RequestId = s.RequestId,
                TableId = s.TableId,
                RequestTime = s.RequestTime,
                Title = s.Title,
                RequestNote = s.RequestNote,
                Code = s.Code,
                Tables = _context.Tables.Where(a => a.TableId == s.TableId)
                                        .Select(o=> new TablesDTO
                                        {
                                            TableId = o.TableId,
                                            TableName = o.TableName,
                                            Note = o.Note,
                                            QR_id = o.QR_id,
                                            Code = o.Code

                                        }).FirstOrDefault()??new TablesDTO(),

            }).ToList();
            return (totalItems, totalPages, requestModel);
        }

        public async Task<bool> UpdateRequestAsync(int requestId, UpdatedRequirementDTO updatedRequirement)
        {
            try
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request == null)
                {
                    throw new Exception("Không tìm thấy yêu cầu");
                }

                if (request.Code != Constants.REQUEST_INIT)
                {
                    throw new Exception("Trạng thái yêu cầu không phải là mới nhất");
                }

                request.Title = updatedRequirement.Title;
                request.RequestNote = updatedRequirement.RequestNote;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi: {ex.Message}");
            }
        }
    }
}
