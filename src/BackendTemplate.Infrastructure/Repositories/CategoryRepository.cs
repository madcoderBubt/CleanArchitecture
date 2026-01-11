using BackendTemplate.Domain.Entities;
using BackendTemplate.Domain.Interfaces;
using BackendTemplate.Infrastructure.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackendTemplate.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context, IConfiguration configuration)
        : base(context, configuration)
    {
    }

    public async Task<Category?> GetCategoryWithProductsAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        // Use EF Core for complex navigation properties
        return await _dbSet
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == categoryId && !c.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var query = @"
            SELECT 
                c.*,
                COUNT(p.Id) as ProductCount
            FROM Categories c
            LEFT JOIN Products p ON c.Id = p.CategoryId AND p.IsDeleted = 0
            WHERE c.IsDeleted = 0
            GROUP BY c.Id, c.Name, c.Description, c.CreatedAt, c.UpdatedAt, c.IsDeleted
            ORDER BY c.Name";
        
        return await connection.QueryAsync<Category>(query);
    }
}
