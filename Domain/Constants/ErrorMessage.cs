using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Constants
{
    public static class ErrorMessage
    {
        public const string ERROR_PASSWORD = "Password must have at least 8 characters, at least one uppercase letter, one lowercase letter, one number and one special character";
        public const string ERROR_CONFIRM_PASSWORD_NOT_MATCH = "The confirm password does not match the password.";
        public const string ERROR_EMAIL_ALREADY_EXISTS = "The email address is already registered.";

    }
}
