using Shared.Constants;

namespace UserService.Extensions;

public static class HttpResponseExtensions
{
    public static void AddUserHeader(this HttpResponse response, Guid userId)
    {
        response.Headers.Append(WellKnownNames.UserIdHeader, userId.ToString());
    }
}