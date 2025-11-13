using UserService.DTOs;

namespace UserService.Services;

public interface IUserService
{
    Task<UserAuthDto> RegisterAsync(InputUserDto registerRequest, CancellationToken cancellationToken);
    Task<IEnumerable<UserAuthDto>> GetUsers(GetUsersRequest request, CancellationToken cancellationToken);
    Task<UserAuthDto> GetUserById(Guid id, CancellationToken cancellationToken);
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
}