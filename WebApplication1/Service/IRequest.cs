using OrderRestaurant.DTO.RequirementDTO;
using OrderRestaurant.Model;

namespace OrderRestaurant.Service
{
    public interface IRequest
    {
        Task<List<RequirementModel>> GetAllRequestsAsync();
        Task<RequirementModel> GetRequestByIdAsync(int id);
        Task<int> CreateRequestAsync(CreatedRequirementDTO requirement);
        Task<bool> CompleteRequestAsync(int requestId);
        Task<bool> RefuseRequestAsync(int requestId);
        Task<bool> DeleteRequestAsync(int id);
        Task<bool> UpdateRequestAsync(int requestId, UpdatedRequirementDTO updatedRequirement);
    }
}
