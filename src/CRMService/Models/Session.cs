using CRMService.DTOs.Session;

namespace CRMService.Models;

public class Session
{
    public required Guid Id { get; set; }
    public required Guid ClientId { get; set; }
    public required DateTime ScheduledAt { get; set; }
    public required int DurationInMinutes { get; set; }
    public required Status Status { get; set; }
    public required string Notes { get; set; }
    
    public Client Client { get; set; }
}

public static class SessionExtensions
{
    public static void Change(this Session currentSession, Session newSession)
    {
        currentSession.ClientId = newSession.ClientId;
        currentSession.ScheduledAt = newSession.ScheduledAt;
        currentSession.DurationInMinutes = newSession.DurationInMinutes;
        currentSession.Status = newSession.Status;
        currentSession.Notes = newSession.Notes;
        currentSession.Client = newSession.Client;
    }
    
    public static Session Update(this Session currentSession, UpdateSessionRequestDto updateDto)
    {
        currentSession.ClientId = updateDto.ClientId;
        currentSession.ScheduledAt = updateDto.ScheduledAt;
        currentSession.DurationInMinutes = updateDto.DurationInMinutes;
        currentSession.Status = updateDto.Status;
        currentSession.Notes = updateDto.Notes;
        
        return currentSession;
    }
}