using CRMService.DTOs;
using CRMService.DTOs.Client;
using CRMService.Services.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Events;
using Shared.Messaging;

namespace CRMService.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<ClientsController> _logger;

    public ClientsController(IClientService clientService,
        IEventPublisher eventPublisher,
        ILogger<ClientsController> logger)
    {
        _clientService = clientService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateClientRequestDto requestDto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new client");
        var result = await _clientService.CreateAsync(requestDto, cancellationToken);

        _logger.LogInformation("Attempt to publish a client creating message");
        try
        {
            var clientCreatedEvent = new ClientCreatedEvent
            {
                ClientId = result.Id,
                Name = result.Name,
                Email = result.Email
            };
            await _eventPublisher.PublishAsync(clientCreatedEvent, RoutingKeys.ClientCreated, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Couldn't publish a client creating message. Error: {error}",
                e.GetBaseException().Message);
        }

        return Created($"api/clients/{result.Id}", result);
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        UpdateClientRequestDto request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating the client");
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
    public async Task<IActionResult> GetClientAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting the client by id");
        var result = await _clientService.GetAsync(id, cancellationToken);

        return Ok(result);
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetClientsByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting clients by the user id");
        var result = await _clientService.GetClientsByUserIdAsync(userId, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteClientAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting the client by id");
        await _clientService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}