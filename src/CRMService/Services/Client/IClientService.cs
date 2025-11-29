using CRMService.DTOs.Client;

namespace CRMService.Services.Client;

public interface IClientService
{
    Task<ClientResponseDto> CreateAsync(CreateClientRequestDto request, Guid userId, CancellationToken cancellationToken);
    Task<ClientResponseDto> UpdateAsync(Guid clientId, Guid userId, UpdateClientRequestDto request, CancellationToken cancellationToken);
    Task<ClientResponseDto> GetAsync(Guid clientId, Guid userId, CancellationToken cancellationToken);
    Task<List<ClientResponseDto>> GetAllUsingPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<List<ClientResponseDto>> GetClientsByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);
}