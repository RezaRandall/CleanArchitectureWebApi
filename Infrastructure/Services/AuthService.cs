using Application.Interfaces;
using Domain.Models;
using Infrastructure.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly ICachingService _cachingService;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, ICachingService cachingService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecurityKey"]));
        _cachingService = cachingService;
    }

    public async Task<AuthResponse> AuthenticateAsync(LoginModel model)
    {
        var user = await _userRepository.GetUserByEmailOrUsername(model.UsernameOrEmail);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            throw new ApplicationException("Invalid username/email or password.");
        }

        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        //await _cachingService.SetAsync(userId.ToString(), refreshToken);
        await _cachingService.SetAsync(user.Id.ToString(), refreshToken);

        // Save refresh token to user (you need to implement this in your repository)
        await _cachingService.SetAsync(user.Id.ToString(), refreshToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }


    private string GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["AccessTokenExpirationInMinutes"])),
            SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        // Generate your refresh token here
        var randomNumber = new byte[32];// 256 bits key
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
