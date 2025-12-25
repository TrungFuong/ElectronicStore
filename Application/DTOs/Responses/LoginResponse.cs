using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AccountId { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public DateTime AccessTokenExpireAt { get; set; }
    }
}
