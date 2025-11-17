using Mapster;
using UserService.Data.Repository;
using UserService.DTOs;
using UserService.Helpers;
using UserService.Models;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserAuthDto> RegisterAsync(RegisterUserRequestDto registerRequest, CancellationToken cancellationToken)
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