namespace Ca.Backend.Test.Application.Services.Interfaces;

public interface IPasswordHasherServices
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}
