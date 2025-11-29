using Mapster;
using UserService.Data.Repository;
using UserService.DTOs;
using UserService.Exceptions;
using UserService.Helpers;
using UserService.Helpers.Jwt;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IUserRepository userRepository,
        IJwtProvider jwtProvider
    )
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<UserAuthDto> RegisterAsync(RegisterUserRequestDto registerRequest,
        CancellationToken cancellationToken)
    {
        var hashedPassword = PasswordHasher.Generate(registerRequest.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = registerRequest.UserName,
            PasswordHash = hashedPassword,
            Email = registerRequest.Email
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var dto = user.Adapt<UserAuthDto>();
        return dto;
    }

    public async Task<string> LoginAsync(LoginUserRequestDto loginRequest, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequest.Email, cancellationToken);
        if (user is null)
            throw new UserNotFoundException("User not found");

        var isValid = PasswordHasher.Verify(loginRequest.Password, user.PasswordHash);
        if (!isValid)
            throw new InvalidDataException("Invalid password");

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