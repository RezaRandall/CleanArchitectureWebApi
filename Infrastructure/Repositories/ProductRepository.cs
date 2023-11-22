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
        //string query = "SELECT * FROM Products";
        string query = "SET NOCOUNT ON " +
            "SELECT * FROM Products WITH (NOLOCK) " + 
            "SET NOCOUNT OFF ";
        return (await _dbConnection.QueryAsync<Product>(query)).AsList();
    }

    public async Task<Product> GetProductByName(string name)
    {
        //string query = "SELECT * FROM Products WHERE Name LIKE '%' + @Name + '%'";
        string query = "SET NOCOUNT ON " +
            "SELECT * " +
            "FROM Ordering_DB.dbo.Products WITH (NOLOCK) " +
            "WHERE Name LIKE '%' + @Name + '%' " +
            "SET NOCOUNT OFF ";
        return await _dbConnection.QueryFirstOrDefaultAsync<Product>(query, new { Name = name });

    }

    public async Task<int> CreateProduct(Product product)
    {
        string query = "SET NOCOUNT ON " +
            "DECLARE @Date DATETIME " + 
            "SET @Date = GETDATE() "+
            "INSERT INTO Ordering_DB.dbo.Products (Name, Price, CreatedAt, UpdatedAt) " +
            " VALUES (@Name, @Price, @Date, '1999-01-01'); SELECT CAST(SCOPE_IDENTITY() as int) " + 
            "SET NOCOUNT OFF ";
        return await _dbConnection.ExecuteScalarAsync<int>(query, product);
    }
    public async Task<bool> UpdateProduct(Product product)
    {
        string query = "SET NOCOUNT ON " +
            "DECLARE @Date DATETIME " +
            "SET @Date = GETDATE() " +
            "UPDATE Ordering_DB.dbo.Products " +
            "SET Name = @Name, Price = @Price, UpdatedAt = @Date " +
            "WHERE Id = @Id " + 
            "SET NOCOUNT OFF ";
        int rowsAffected = await _dbConnection.ExecuteAsync(query, product);
        return rowsAffected > 0;        
    }

    public async Task<bool> DeleteProduct(int productId)
    {
        string query = "SET NOCOUNT ON " +
            "DELETE FROM Ordering_DB.dbo.Products WITH (NOLOCK) WHERE Id = @Id " +
            "SET NOCOUNT OFF ";
        int rowsAffected = await _dbConnection.ExecuteAsync(query, new { Id = productId });
        return rowsAffected > 0;
    }

}
