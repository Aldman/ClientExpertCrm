using CRMService.Data.Repositories.Client;
using CRMService.DTOs.Client;
using CRMService.Exceptions;
using CRMService.Models;
using CRMService.Validators;
using Mapster;

namespace CRMService.Services.Client;

public class ClientService : IClientService
{
    private readonly IClientRepository _repository;

    public ClientService(IClientRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ClientResponseDto> CreateAsync(
        CreateClientRequestDto request, 
        Guid userId,
        CancellationToken cancellationToken)
    {
        var client = request.Adapt<Models.Client>();
        client.Id = Guid.NewGuid();
        client.UserId = userId;
        
        await _repository.AddAsync(client, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        
        return client.Adapt<ClientResponseDto>();
    }

    public async Task<ClientResponseDto> UpdateAsync(Guid clientId, Guid userId, UpdateClientRequestDto request, CancellationToken cancellationToken)
    {
        var client = await _repository.GetAsync(clientId, cancellationToken);
        if (client is null)
            throw new ClientNotFoundException("Client not found");
        CommonValidators.ValidateOrThrow(userId, client.UserId);
        
        client = client.Update(request);
        await _repository.UpdateAsync(client, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        
        return client.Adapt<ClientResponseDto>();
    }

    public async Task<ClientResponseDto> GetAsync(Guid clientId, Guid userId, CancellationToken cancellationToken)
    {
        var client = await _repository.GetAsync(clientId, cancellationToken);
        if (client is not null)
            CommonValidators.ValidateOrThrow(userId, client.UserId);
        
        return client.Adapt<ClientResponseDto>();
    }
    
    public async Task<List<ClientResponseDto>> GetAllUsingPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var clients = await _repository.GetClientsByPaginationAsync(page, pageSize, cancellationToken);
        
        return clients.Adapt<List<ClientResponseDto>>();
    }

    public async Task<List<ClientResponseDto>> GetClientsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var clients = await _repository.GetClientsByUserIdAsync(userId, cancellationToken);
        
        return clients.Adapt<List<ClientResponseDto>>();
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        await GetAsync(id, userId, cancellationToken);
        await  _repository.DeleteAsync(id, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}