namespace UserService.Helpers;

public static class PasswordHasher
{
    public static string Generate(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public static bool Verify(string inputPassword, string hashedPassword) =>
        BCrypt.Net.BCrypt.EnhancedVerify(inputPassword, hashedPassword);
}