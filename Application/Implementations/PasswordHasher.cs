using Application.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32); // 256 bits

            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string salt, string hash)
        {
            var hashToCheck = HashPassword(password, salt);
            return hashToCheck == hash;
        }
    }
}
