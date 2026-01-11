using AutoMapper;
using BackendTemplate.Application.DTOs;
using BackendTemplate.Application.Services;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace BackendTemplate.Tests.Unit.Services;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsSuccessResult()
    {
        // Arrange
        var productId = 1;
        var product = new Product { Id = productId, Name = "Test Product" };
        var productDto = new ProductDto { Id = productId, Name = "Test Product" };

        _mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        _mockMapper.Setup(m => m.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(productDto);
        result.Message.Should().Be("Product retrieved successfully");
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var productId = 1;

        _mockUnitOfWork.Setup(u => u.Products.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetByIdAsync(productId);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Product not found");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryExists_ReturnsSuccessResult()
    {
        // Arrange
        var request = new CreateProductRequest 
        { 
            Name = "New Product", 
            CategoryId = 1,
            Price = 100,
            Stock = 10
        };
        var product = new Product { Id = 1, Name = "New Product", CategoryId = 1 };
        var productDto = new ProductDto { Id = 1, Name = "New Product", CategoryId = 1 };

        _mockUnitOfWork.Setup(u => u.Categories.ExistsAsync(request.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _mockMapper.Setup(m => m.Map<Product>(request))
            .Returns(product);
        
        _mockUnitOfWork.Setup(u => u.Products.AddAsync(product, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        _mockMapper.Setup(m => m.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _productService.CreateAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeEquivalentTo(productDto);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var request = new CreateProductRequest { CategoryId = 99 };

        _mockUnitOfWork.Setup(u => u.Categories.ExistsAsync(request.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _productService.CreateAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid category");
        _mockUnitOfWork.Verify(u => u.Products.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
