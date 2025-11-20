using CRMService.DTOs.Client;

namespace CRMService.Services.Client;

public interface IClientService
{
    Task<ClientResponseDto> CreateAsync(CreateClientRequestDto request, CancellationToken cancellationToken);
    Task<ClientResponseDto> UpdateAsync(Guid clientId, UpdateClientRequestDto request, CancellationToken cancellationToken);
    Task<ClientResponseDto> GetAsync(Guid clientId, CancellationToken cancellationToken);
    Task<List<ClientResponseDto>> GetAllUsingPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<List<ClientResponseDto>> GetClientsByUserId(Guid userId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}