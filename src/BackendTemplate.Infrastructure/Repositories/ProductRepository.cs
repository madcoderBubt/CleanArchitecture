using BackendTemplate.Domain.Entities;
using BackendTemplate.Domain.Interfaces;
using BackendTemplate.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace BackendTemplate.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context, IConfiguration configuration)
        : base(context, configuration)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var query = @"
            SELECT p.*, c.Name as CategoryName
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.CategoryId = @CategoryId AND p.IsDeleted = 0
            ORDER BY p.Name";
        
        return await connection.QueryAsync<Product>(query, new { CategoryId = categoryId });
    }

    public async Task<IEnumerable<Product>> GetProductsInStockAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var query = @"
            SELECT p.*, c.Name as CategoryName
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE p.Stock > 0 AND p.IsDeleted = 0
            ORDER BY p.Name";
        
        return await connection.QueryAsync<Product>(query);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var query = @"
            SELECT p.*, c.Name as CategoryName
            FROM Products p
            LEFT JOIN Categories c ON p.CategoryId = c.Id
            WHERE (p.Name LIKE @SearchTerm OR p.Description LIKE @SearchTerm) 
                AND p.IsDeleted = 0
            ORDER BY p.Name";
        
        var searchPattern = $"%{searchTerm}%";
        return await connection.QueryAsync<Product>(query, new { SearchTerm = searchPattern });
    }
}
