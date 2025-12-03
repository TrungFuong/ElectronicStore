using Domain.Enums;

namespace Application.DTOs.Requests
{
    public class RegisterRequest
    {
        public string Phone { get; set; }
        public string Password { get; set; }
        public EnumRole Role { get; set; } = EnumRole.Customer;
    }
}
