using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Messaging;
using UserService.DTOs;
using UserService.Services;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEventPublisher _eventPublisher;

    public AuthController(IUserService userService, IEventPublisher eventPublisher)
    {
        _userService = userService;
        _eventPublisher = eventPublisher;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserAuthDto>> Register(
        [FromBody] InputUserDto user,
        CancellationToken cancellationToken)
    {
        var result = await _userService.RegisterAsync(user, cancellationToken);
        await _eventPublisher.PublishAsync(result, RoutingKeys.ClientCreated, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetUserById),
            routeValues: new { UserId = result.Id },
            value: result
        );
    }

    [HttpGet("users")]
    public async Task<ActionResult<IEnumerable<UserAuthDto>>> GetUsers(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _userService.GetUsers(request, cancellationToken);

        return Ok(result);
    }

    [HttpGet("user/{userId:guid}")]
    [ActionName(nameof(GetUserById))]
    public async Task<ActionResult<UserAuthDto>> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserById(userId, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("delete/{userId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(userId, cancellationToken);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserAuthDto>> Login([FromBody] InputUserDto user)
    {
        return Ok();
    }
}