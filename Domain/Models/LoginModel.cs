namespace Domain.Models;

public class LoginModel
{
    public string UsernameOrEmail { get; set; }
    public string Password { get; set; }
}

public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}