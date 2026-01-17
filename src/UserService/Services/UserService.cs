using Mapster;
using Shared.Constants;
using UserService.Data.Repositories.Outbox;
using UserService.Data.Repositories.User;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Helpers;
using UserService.Helpers.Jwt;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository,
        IOutboxRepository outboxRepository,
        IJwtProvider jwtProvider,
        ILogger<UserService> logger
    )
    {
        _userRepository = userRepository;
        _outboxRepository = outboxRepository;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<UserAuthDto> RegisterAsync(RegisterUserRequestDto registerRequest,
        CancellationToken cancellationToken)
    {
        var hashedPassword = PasswordHasher.Generate(registerRequest.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = registerRequest.UserName!,
            PasswordHash = hashedPassword,
            Email = registerRequest.Email
        };

        await using var transaction = await _userRepository.CreateTransactionAsync(cancellationToken);
        try
        {
            await _userRepository.AddAsync(user, cancellationToken);
            await _userRepository.SaveChangesAsync(cancellationToken);
                
            _logger.LogInformation("Attempt to publish a registration message");
                
            var dto = user.Adapt<UserAuthDto>();
            await _outboxRepository.AddContentMessageAsync(
                message: dto,
                routingKey: RoutingKeys.UserCreated,
                cancellationToken);
            await _outboxRepository.SaveChangesAsync(cancellationToken); 
            
            await transaction.CommitAsync(cancellationToken);
            return dto;
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError("Couldn't register a new user. Error: {error}", e.GetBaseException().Message);
            throw;
        }
    }

    public async Task<string> LoginAsync(LoginUserRequestDto loginRequest, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequest.Email, cancellationToken);
        if (user is null)
            throw new UserNotFoundException("User not found");

        var isValid = PasswordHasher.Verify(loginRequest.Password, user.PasswordHash);
        if (!isValid)
            throw new InvalidDataException("Invalid password");
        
        _logger.LogInformation("Attempt to publish a log in message");
        try
        {
            await _outboxRepository.AddContentMessageAsync(
                message: user.Email!,
                routingKey: RoutingKeys.UserLoggedIn,
                cancellationToken);
            await _outboxRepository.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Couldn't publish log in message. Error: {error}", e.GetBaseException().Message);
        }

        var token = _jwtProvider.GenerateJwtToken(user);

        return token;
    }

    public async Task<IEnumerable<UserAuthDto>> GetUsers(
        GetUsersRequest request,
        CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetUsersAsync(request.Page, request.PageSize, cancellationToken);
        return users.Adapt<IEnumerable<UserAuthDto>>();
    }

    public async Task<UserAuthDto> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        return user.Adapt<UserAuthDto>();
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _userRepository.DeleteAsync(userId, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}