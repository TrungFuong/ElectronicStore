using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAdminAccountService
    {
        Task<bool> AddStaffAsync(AddStaffRequest request);
        Task<IEnumerable<StaffResponse>> GetAllStaffAsync();
        Task<StaffResponse?> GetByIdAsync(string staffId);
        Task<bool> UpdateStaffAsync(UpdateStaffRequest request);
        Task<bool> DeleteStaffAsync(string staffId);
        Task<bool> SetStaffStatusAsync(string staffId, bool isActive);
    }
}