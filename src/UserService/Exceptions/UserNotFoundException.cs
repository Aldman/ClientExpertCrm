namespace UserService.Exceptions;

public class UserNotFoundException(string message) : Exception(message);