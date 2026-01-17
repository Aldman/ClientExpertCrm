using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public AuthController(IUserService userService,
        ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserAuthDto>> Register(
        [FromBody] RegisterUserRequestDto userRequest,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("User registration");
        var result = await _userService.RegisterAsync(userRequest, cancellationToken);

        _logger.LogInformation("Adding user id to headers");
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
        _logger.LogInformation("Getting user by id");
        var result = await _userService.GetUserById(userId, cancellationToken);
        
        _logger.LogInformation("Adding user id to headers");
        Response.AddUserHeader(result.Id);

        return Ok(result);
    }

    [Authorize]
    [HttpDelete("delete/{userId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting a user by id");
        await _userService.DeleteAsync(userId, cancellationToken);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(
        [FromBody] LoginUserRequestDto userRequest,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempt to log in");
        var token = await _userService.LoginAsync(userRequest, cancellationToken);
        
        return Ok(token);
    }
}