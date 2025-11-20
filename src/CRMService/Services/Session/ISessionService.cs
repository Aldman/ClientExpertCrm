using CRMService.DTOs.Session;

namespace CRMService.Services.Session;

public interface ISessionService
{
    Task<SessionResponseDto> CreateAsync(CreateSessionRequestDto request, CancellationToken cancellationToken);
    Task<SessionResponseDto> UpdateAsync(Guid id, UpdateSessionRequestDto request, CancellationToken cancellationToken);
    Task<SessionResponseDto> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<List<SessionResponseDto>> GetAllUsingPaginationAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<List<SessionResponseDto>> GetSessionsByClientId(Guid clientId, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}