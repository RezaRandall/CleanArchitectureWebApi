using Application.Interfaces;
using Dapper;
using Domain.Models;
using System.Data;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly IDbConnection _dbConnection;

    public ProductRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }
    public async Task<List<Product>> GetAllProducts()
    {
        string query = "SELECT * FROM Products";
        return (await _dbConnection.QueryAsync<Product>(query)).AsList();
    }

    public async Task<Product> GetProductByName(string name)
    {
        string query = "SELECT * FROM Products WHERE Name LIKE '%' + @Name + '%'";
        return await _dbConnection.QueryFirstOrDefaultAsync<Product>(query, new { Name = name });

    }

    public async Task<int> CreateProduct(Product product)
    {
        string query = "INSERT INTO Products (Name, Price, CreatedAt, UpdatedAt) VALUES (@Name, @Price, GETDATE(), GETDATE()); SELECT CAST(SCOPE_IDENTITY() as int)";
        return await _dbConnection.ExecuteScalarAsync<int>(query, product);
    }
    public async Task<bool> UpdateProduct(Product product)
    {
        string query = "UPDATE Products SET Name = @Name, Price = @Price, UpdatedAt = @GETDATE() WHERE Id = @Id";
        int rowsAffected = await _dbConnection.ExecuteAsync(query, product);
        return rowsAffected > 0;        
    }

    public async Task<bool> DeleteProduct(int productId)
    {
        string query = "DELETE FROM Products WHERE Id = @Id";
        int rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = productId });
        return rowsAffected > 0;
    }

}
