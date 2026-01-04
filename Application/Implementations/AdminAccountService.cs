using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                Email = request.Email,
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
                // operational status for staff is tracked on Staff.IsActive
                IsActive = true
            };

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.AccountRepository.AddAsync(account);
                await _unitOfWork.StaffRepository.AddAsync(staff);
            });
            return true;
        }

        public async Task<IEnumerable<StaffResponse>> GetAllStaffAsync()
        {
            var staffs = await _unitOfWork.StaffRepository.GetAllAsync(null, s => s.Account);
            return staffs.Select(s => new StaffResponse
            {
                StaffId = s.StaffId,
                StaffName = s.StaffName,
                Phone = s.Phone,
                StaffDOB = s.StaffDOB,
                AccountId = s.AccountId,
                // Use Staff.IsActive as the canonical operational status for staff
                IsActive = s.IsActive
            });
        }

        public async Task<StaffResponse?> GetByIdAsync(string staffId)
        {
            var s = await _unitOfWork.StaffRepository.GetAsync(x => x.StaffId == staffId, x => x.Account);
            if (s == null) return null;
            return new StaffResponse
            {
                StaffId = s.StaffId,
                StaffName = s.StaffName,
                Phone = s.Phone,
                StaffDOB = s.StaffDOB,
                AccountId = s.AccountId,
                // reflect Staff.IsActive
                IsActive = s.IsActive
            };
        }

        public async Task<bool> UpdateStaffAsync(UpdateStaffRequest request)
        {
            var staff = await _unitOfWork.StaffRepository.GetAsync(s => s.StaffId == request.StaffId, s => s.Account);
            if (staff == null) return false;

            if (!string.Equals(staff.Phone, request.Phone, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _unitOfWork.AccountRepository.GetByPhoneAsync(request.Phone);
                if (exists != null && exists.AccountId != staff.AccountId)
                    throw new Exception("Số điện thoại đã được sử dụng bởi tài khoản khác");
            }

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                staff.StaffName = request.StaffName;
                staff.Phone = request.Phone;
                staff.StaffDOB = request.StaffDOB;

                // If client provided IsActive, apply it to both Staff and Account
                if (request.IsActive.HasValue)
                {
                    staff.IsActive = request.IsActive.Value;

                    if (staff.Account != null)
                    {
                        staff.Account.IsActive = request.IsActive.Value;
                        _unitOfWork.AccountRepository.Update(staff.Account);
                    }
                }

                _unitOfWork.StaffRepository.Update(staff);

                if (staff.Account != null)
                {
                    staff.Account.Phone = request.Phone;
                    // account update already handled above when IsActive changed; ensure phone persisted
                    _unitOfWork.AccountRepository.Update(staff.Account);
                }
            });

            return true;
        }

        public async Task<bool> SetStaffStatusAsync(string staffId, bool isActive)
        {
            var staff = await _unitOfWork.StaffRepository.GetAsync(s => s.StaffId == staffId, s => s.Account);
            if (staff == null) return false;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                staff.IsActive = isActive;
                _unitOfWork.StaffRepository.Update(staff);

                if (staff.Account != null)
                {
                    staff.Account.IsActive = isActive;
                    _unitOfWork.AccountRepository.Update(staff.Account);
                }
            });

            return true;
        }

        public async Task<bool> DeleteStaffAsync(string staffId)
        {
            var staff = await _unitOfWork.StaffRepository.GetAsync(s => s.StaffId == staffId, s => s.Account);
            if (staff == null) return false;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // mark staff as inactive (soft-delete)
                staff.IsActive = false;
                _unitOfWork.StaffRepository.Update(staff);

                // also soft-disable associated account so staff cannot login
                if (staff.Account != null)
                {
                    staff.Account.IsActive = false;
                    _unitOfWork.AccountRepository.Update(staff.Account);
                }
            });

            return true;
        }

    }
}
