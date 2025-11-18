using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Messaging;
using UserService.DTOs;
using UserService.Extensions;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;
    private readonly IEventPublisher _eventPublisher;

    public AuthController(IUserService userService,
        ILogger<AuthController> logger,
        IEventPublisher eventPublisher)
    {
        _userService = userService;
        _logger = logger;
        _eventPublisher = eventPublisher;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserAuthDto>> Register(
        [FromBody] RegisterUserRequestDto userRequest,
        CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(userRequest, cancellationToken);

        try
        {
            await _eventPublisher.PublishAsync(result, RoutingKeys.ClientCreated, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Couldn't publish register message. Error: {error}", e.GetBaseException().Message);
        }

        Response.AddUserHeader(result.Id);

        return CreatedAtAction(
            actionName: nameof(GetUserById),
            routeValues: new { UserId = result.Id },
            value: result
        );
    }

    [Authorize]
    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserAuthDto>>> GetUsers(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.GetUsers(request, cancellationToken);

        return Ok(result);
    }

    [Authorize]
    [HttpGet("user/{userId:guid}")]
    [ActionName(nameof(GetUserById))]
    public async Task<ActionResult<UserAuthDto>> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserById(userId, cancellationToken);
        Response.AddUserHeader(result.Id);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(userId, cancellationToken);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(
        [FromBody] LoginUserRequestDto userRequest,
        CancellationToken cancellationToken)
    {
        var token = await _userService.LoginAsync(userRequest, cancellationToken);

        Response.Cookies.Append(WellKnownNames.TokenName, token);

        return Ok(token);
    }
}