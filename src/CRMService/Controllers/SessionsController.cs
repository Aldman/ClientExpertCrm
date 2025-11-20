using CRMService.DTOs;
using CRMService.DTOs.Session;
using CRMService.Services.Session;
using Microsoft.AspNetCore.Mvc;

namespace CRMService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(ISessionService sessionService, ILogger<SessionsController> logger)
    {
        _sessionService = sessionService;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateSessionRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        var result = await _sessionService.CreateAsync(requestDto, cancellationToken);

        return Created($"api/sessions/{result.Id}", result);
    }

    [HttpPost("{id:guid}")]
    public async Task<ActionResult<SessionResponseDto>> UpdateAsync(
        Guid id,
        UpdateSessionRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _sessionService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }
    
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SessionResponseDto>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sessionService.GetAsync(id, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<SessionResponseDto>>> GetAllUsingPaginationAsync(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _sessionService
            .GetAllUsingPaginationAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("client/{clientId:guid}")]
    public async Task<ActionResult<List<SessionResponseDto>>> GetSessionsByClientId(
        Guid clientId,
        CancellationToken cancellationToken)
    {
        var result = await _sessionService.GetSessionsByClientId(clientId, cancellationToken);
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _sessionService.DeleteAsync(id, cancellationToken);
        
        return NoContent();
    }
}