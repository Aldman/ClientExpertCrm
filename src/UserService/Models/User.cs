namespace UserService.Models;

public class User
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string PasswordHash { get; set; }
    public string? Email { get; set; }
}

public static class UserExtensions
{
    public static void Change(this User currentUser, User newUser)
    {
        currentUser.Email = newUser.Email;
        currentUser.PasswordHash = newUser.PasswordHash;
        currentUser.UserName = newUser.UserName;
    }
}