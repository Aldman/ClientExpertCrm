using CRMService.Data.Repositories.Session;
using CRMService.DTOs.Session;
using CRMService.Exceptions;
using CRMService.Models;
using Mapster;

namespace CRMService.Services.Session;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _repository;

    public SessionService(ISessionRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<SessionResponseDto> CreateAsync(CreateSessionRequestDto request, CancellationToken cancellationToken)
    {
        var session = request.Adapt<Models.Session>();
        session.Id = Guid.NewGuid();
        session.Status = Status.Planned;
        
        await _repository.AddAsync(session, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return session.Adapt<SessionResponseDto>();
    }

    public async Task<SessionResponseDto> UpdateAsync(Guid id, UpdateSessionRequestDto request, CancellationToken cancellationToken)
    {
        var session = await _repository.GetAsync(id, cancellationToken);
        if (session == null)
        {
            throw new SessionNotFoundException("Session not found");
        }

        session = session.Update(request);
        await _repository.UpdateAsync(session, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        
        return session.Adapt<SessionResponseDto>();
    }

    public async Task<SessionResponseDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var session = await _repository.GetAsync(id, cancellationToken);
        
        return session.Adapt<SessionResponseDto>();
    }

    public async Task<List<SessionResponseDto>> GetAllUsingPaginationAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var sessions = await _repository.GetSessionsByPaginationAsync(page, pageSize, cancellationToken);
        
        return sessions.Adapt<List<SessionResponseDto>>();
    }

    public async Task<List<SessionResponseDto>> GetSessionsByClientId(Guid clientId, CancellationToken cancellationToken)
    {
        var sessions = await _repository.GetSessionsByClientIdAsync(clientId, cancellationToken);
        
        return sessions.Adapt<List<SessionResponseDto>>();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await  _repository.DeleteAsync(id, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}