namespace CRMService.Extensions;

public static class StringExtensions
{
    public static Guid ToGuid(this string value) => Guid.Parse(value);
}