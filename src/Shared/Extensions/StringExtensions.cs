using System.Text.Json;
using Shared.Events;

namespace Shared.Extensions;

public static class StringExtensions
{
    public static ClientCreatedEvent? ToClientCreatedEvent(this string json)
    {
        try
        {
            return JsonSerializer.Deserialize<ClientCreatedEvent>(json);
        }
        catch
        {
            return null;
        }
    }
    
    public static SessionPlannedEvent? ToSessionPlannedEvent(this string json)
    {
        try
        {
            return JsonSerializer.Deserialize<SessionPlannedEvent>(json);
        }
        catch
        {
            return null;
        }
    }
}