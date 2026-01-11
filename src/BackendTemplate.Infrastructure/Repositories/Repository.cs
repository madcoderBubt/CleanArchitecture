using System.Data;
using System.Linq.Expressions;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Domain.Interfaces;
using BackendTemplate.Infrastructure.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackendTemplate.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly string _connectionString;

    public Repository(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
    }

    protected IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    // Read operations using Dapper for performance
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var tableName = typeof(T).Name + "s";
        var query = $"SELECT * FROM {tableName} WHERE Id = @Id AND IsDeleted = 0";
        return await connection.QueryFirstOrDefaultAsync<T>(query, new { Id = id });
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var tableName = typeof(T).Name + "s";
        var query = $"SELECT * FROM {tableName} WHERE IsDeleted = 0";
        return await connection.QueryAsync<T>(query);
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        // For complex queries, fall back to EF Core
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var tableName = typeof(T).Name + "s";
        
        var countQuery = $"SELECT COUNT(*) FROM {tableName} WHERE IsDeleted = 0";
        var totalCount = await connection.ExecuteScalarAsync<int>(countQuery);

        var offset = (pageNumber - 1) * pageSize;
        var query = $@"
            SELECT * FROM {tableName} 
            WHERE IsDeleted = 0 
            ORDER BY Id 
            OFFSET @Offset ROWS 
            FETCH NEXT @PageSize ROWS ONLY";
        
        var items = await connection.QueryAsync<T>(query, new { Offset = offset, PageSize = pageSize });
        
        return (items, totalCount);
    }

    // Write operations using EF Core
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            // Soft delete
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        using var connection = CreateConnection();
        var tableName = typeof(T).Name + "s";
        var query = $"SELECT COUNT(1) FROM {tableName} WHERE Id = @Id AND IsDeleted = 0";
        var count = await connection.ExecuteScalarAsync<int>(query, new { Id = id });
        return count > 0;
    }
}
