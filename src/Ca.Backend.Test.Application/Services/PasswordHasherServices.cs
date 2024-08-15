using System.Security.Cryptography;
using System.Text;
using Ca.Backend.Test.Application.Services.Interfaces;

namespace Ca.Backend.Test.Application.Services;

public class PasswordHasherServices : IPasswordHasherServices
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var hashedInput = HashPassword(password);
        return hashedInput == hashedPassword;
    }
}
