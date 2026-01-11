using BackendTemplate.Domain.Entities;
using BackendTemplate.Infrastructure.Data;
using BackendTemplate.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BackendTemplate.Tests.Integration.Repositories;

public class ProductRepositoryTests
{
    private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public ProductRepositoryTests()
    {
        _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c.GetConnectionString("DefaultConnection"))
            .Returns("Server=(localdb)\\mssqllocaldb;Database=TestDb;Trusted_Connection=true");
    }

    [Fact]
    public async Task AddAsync_ShouldAddProductToDatabase()
    {
        // Arrange
        using var context = new ApplicationDbContext(_dbContextOptions);
        var repository = new ProductRepository(context, _mockConfiguration.Object);
        var product = new Product
        {
            Name = "Integration Test Product",
            Description = "Description",
            Price = 99.99m,
            Stock = 10,
            CategoryId = 1,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await repository.AddAsync(product);
        await context.SaveChangesAsync();

        // Assert
        using var verifyContext = new ApplicationDbContext(_dbContextOptions);
        var savedProduct = await verifyContext.Products.FirstOrDefaultAsync(p => p.Name == "Integration Test Product");
        Assert.NotNull(savedProduct);
        Assert.Equal(product.Price, savedProduct.Price);
    }

    // Note: We cannot easily test Dapper methods (GetByIdAsync, GetAllAsync) with InMemory database
    // as Dapper requires a real SQL connection. For those tests, we would need 
    // a real LocalDB instance or TestContainers.
}
