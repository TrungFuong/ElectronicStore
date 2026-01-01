using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;

namespace Application.Implementations
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public AdminAccountService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> AddStaffAsync(AddStaffRequest request)
        {
            var existed = await _unitOfWork.AccountRepository.GetByPhoneAsync(request.Phone);

            if (existed != null)
                throw new Exception("Số điện thoại đã tồn tại");

            var account = new Account
            {
                AccountId = Guid.NewGuid().ToString(),
                Phone = request.Phone,
                HashPassword = _passwordHasher.HashPassword("NV.12345"),
                Role = EnumRole.Staff,
                IsActive = true
            };


            var staff = new Staff
            {
                StaffId = Prefixes.STAFF_ID_PREFIX + Guid.NewGuid().ToString(),
                AccountId = account.AccountId,
                StaffName = request.StaffName,
                Phone = request.Phone,
                StaffDOB = request.StaffDOB,
            };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.AccountRepository.AddAsync(account);
                await _unitOfWork.StaffRepository.AddAsync(staff);
            });
            return true;
        }
    }
}
