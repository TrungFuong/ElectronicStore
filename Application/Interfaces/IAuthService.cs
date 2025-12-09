using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs.Auth;
using System.Threading.Tasks;
using Domain.Models.Responses;
using Domain.Models.Requests;
using Application.DTOs.Requests;

namespace Application.Interfaces
{

    public interface IAuthService
    {
        //Đăng ký
        Task<bool> RegisterAsync(RegisterRequest request);
        //Đăng nhập
        Task<LoginResponse> LoginAsync(LoginRequest request);

        //Làm mới token
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);

        //Đăng xuất (revoke token)
        Task<bool> LogoutAsync(string refreshToken);

        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);

        // 5. Reset mật khẩu (quên mật khẩu, kèm otp)
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
