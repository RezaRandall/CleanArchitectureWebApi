using Domain.Models;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> AuthenticateAsync(LoginModel model);
}
