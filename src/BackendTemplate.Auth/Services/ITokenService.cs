using BackendTemplate.Domain.Entities;

namespace BackendTemplate.Auth.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
