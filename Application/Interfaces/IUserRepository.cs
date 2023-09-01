using Domain.Models;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task AddUser(User user);
    Task<bool> DoesEmailExist(string email);
    Task<bool> DoesUsernameExist(string username);
    Task<User> GetUserByEmailOrUsername(string usernameOrEmail);
    //Task UpdateRefreshToken(object id, string refreshToken);
}
