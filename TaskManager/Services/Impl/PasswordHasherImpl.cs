using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace TaskManager.Services.Impl
{
    public class PasswordHasherImpl : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
