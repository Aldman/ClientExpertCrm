namespace CRMService.Validators;

public static class CommonValidators
{
    public static void ValidateOrThrow(Guid inputUserId, Guid modelUserId)
    {
        if (modelUserId != inputUserId)
            throw new UnauthorizedAccessException("You can't work with other user's data.");
    }
}