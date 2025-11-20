using CRMService.DTOs;
using CRMService.DTOs.Client;
using CRMService.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace CRMService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientService clientService, ILogger<ClientsController> logger)
    {
        _clientService = clientService;
        _logger = logger;
    }
    
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateClientRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        var result = await _clientService.CreateAsync(requestDto, cancellationToken);
        
        return CreatedAtAction(
            actionName: "GetClient",
            routeValues: new { ClientId = result.Id },
            value: result
        );
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        UpdateClientRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _clientService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] PaginationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _clientService
            .GetAllUsingPaginationAsync(request.PageNumber, request.PageSize, cancellationToken);
        
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    [ActionName("GetClient")]
    public async Task<IActionResult> GetClientAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _clientService.GetAsync(id, cancellationToken);
        
        return Ok(result);
    }
    
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetClientsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await _clientService.GetClientsByUserIdAsync(userId, cancellationToken);
        
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClientAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        await _clientService.DeleteAsync(id, cancellationToken);
        
        return NoContent();
    }
}