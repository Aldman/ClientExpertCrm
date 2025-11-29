using CRMService.Cache;
using CRMService.Data.Repositories.Session;
using CRMService.DTOs.Session;
using CRMService.Exceptions;
using CRMService.Extensions;
using CRMService.Models;
using Mapster;

namespace CRMService.Services.Session;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _persistentRepository;
    private readonly ICacheRepository _cacheRepository;

    public SessionService(ISessionRepository persistentRepository, ICacheRepository cacheRepository)
    {
        _persistentRepository = persistentRepository;
        _cacheRepository = cacheRepository;
    }

    public async Task<SessionResponseDto> CreateAsync(CreateSessionRequestDto request,
        CancellationToken cancellationToken)
    {
        var session = request.Adapt<Models.Session>();
        session.Id = Guid.NewGuid();
        session.Status = Status.Planned;

        await _persistentRepository.AddAsync(session, cancellationToken);
        await _persistentRepository.SaveChangesAsync(cancellationToken);

        var sessionToResponse = session.Adapt<SessionResponseDto>();
        _cacheRepository.AddOrUpdate(sessionToResponse);

        return sessionToResponse;
    }

    public async Task<SessionResponseDto> UpdateAsync(Guid id, UpdateSessionRequestDto request,
        CancellationToken cancellationToken)
    {
        var session = await _persistentRepository.GetAsync(id, cancellationToken);
        if (session == null)
        {
            throw new SessionNotFoundException("Session not found");
        }

        session = session.Update(request);
        await _persistentRepository.UpdateAsync(session, cancellationToken);
        await _persistentRepository.SaveChangesAsync(cancellationToken);

        var sessionToResponse = session.Adapt<SessionResponseDto>();
        _cacheRepository.AddOrUpdate(sessionToResponse);

        return sessionToResponse;
    }

    public async Task<SessionResponseDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var session = await _persistentRepository.GetAsync(id, cancellationToken);

        return session.Adapt<SessionResponseDto>();
    }

    public async Task<List<SessionResponseDto>> GetAllUsingPaginationAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var sessions = await _persistentRepository.GetSessionsByPaginationAsync(page, pageSize, cancellationToken);

        return sessions.Adapt<List<SessionResponseDto>>();
    }

    public async Task<List<SessionResponseDto>> GetSessionsByClientId(Guid clientId,
        CancellationToken cancellationToken)
    {
        var cached = _cacheRepository.GetAllSerialized(clientId.ToHashKey());
        if (cached.Any())
            return cached.ToList();
        
        var sessions = await _persistentRepository.GetSessionsByClientIdAsync(clientId, cancellationToken);

        return sessions.Adapt<List<SessionResponseDto>>();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var session = await _persistentRepository.GetAsync(id, cancellationToken);
        if (session is not null)
            _cacheRepository.DeleteEntity(session.ClientId.ToHashKey(), session.Id.ToSessionCacheKey());
        
        await _persistentRepository.DeleteAsync(id, cancellationToken);
        await _persistentRepository.SaveChangesAsync(cancellationToken);
    }
}