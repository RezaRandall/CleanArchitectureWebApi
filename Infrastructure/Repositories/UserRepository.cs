using Application.Interfaces;
using Dapper;
using Domain.Models;
using System.Data;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnection _dbConnection;

    public UserRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task AddUser(User user)
    {
        // Di sini Anda perlu menulis kode untuk menyimpan data user ke database menggunakan Dapper
        // Contoh:
        var query = "INSERT INTO Users (Email, Username, Password, CreatedAt) VALUES (@Email, @Username, @Password, GETDATE())";
        await _dbConnection.ExecuteAsync(query, user);
    }

    public async Task<bool> DoesEmailExist(string email)
    {
        var query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
        var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Email = email });
        return count > 0;
    }

    public async Task<bool> DoesUsernameExist(string username)
    {
        var query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
        var count = await _dbConnection.ExecuteScalarAsync<int>(query, new { Username = username });
        return count > 0;
    }

    public async Task<User> GetUserByEmailOrUsername(string usernameOrEmail)
    {
        var query = "SELECT * FROM Users WHERE Email = @UsernameOrEmail OR Username = @UsernameOrEmail";
        return await _dbConnection.QueryFirstOrDefaultAsync<User>(query, new { UsernameOrEmail = usernameOrEmail });
    }

    //public Task UpdateRefreshToken(object id, string refreshToken)
    //{
    //    throw new NotImplementedException();
    //}
}
