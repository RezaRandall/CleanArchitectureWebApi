using Application.Interfaces;
using Application.Users.Commands;
using Domain.Models;
using MediatR;

namespace Application.Handlers;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, int>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Check if email or username already exist
        if (await _userRepository.DoesEmailExist(request.Email))
        {
            throw new ApplicationException("Email already exists.");
        }
        if (await _userRepository.DoesUsernameExist(request.Username))
        {
            throw new ApplicationException("Username already exists.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Email = request.Email,
            Username = request.Username,
            Password = hashedPassword
        };

        await _userRepository.AddUser(newUser);

        return newUser.Id;
    }
}
