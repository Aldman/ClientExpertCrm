using CRMService.DTOs.Client;

namespace CRMService.Models;

public class Client
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public List<Session> Sessions { get; set; } = [];
}

public static class ClientExtensions
{
    public static void Change(this Client currentClient, Client newClient)
    {
        currentClient.Name = newClient.Name;
        currentClient.Email = newClient.Email;
        currentClient.Phone = newClient.Phone;
        currentClient.UserId = newClient.UserId;
        currentClient.Sessions = newClient.Sessions;
    }
    
    public static Client Update(this Client currentClient, UpdateClientRequestDto updateInfo)
    {
        currentClient.Name = updateInfo.Name;
        currentClient.Email = updateInfo.Email;
        currentClient.Phone = updateInfo.Phone;
        
        return currentClient;
    }
}