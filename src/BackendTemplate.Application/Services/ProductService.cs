using AutoMapper;
using BackendTemplate.Application.DTOs;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Domain.Interfaces;
using BackendTemplate.Shared.Models;

namespace BackendTemplate.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            
            if (product == null)
                return ApiResponse<ProductDto>.FailureResult("Product not found", $"Product with ID {id} does not exist");

            var productDto = _mapper.Map<ProductDto>(product);
            return ApiResponse<ProductDto>.SuccessResult(productDto, "Product retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.FailureResult("Error retrieving product", ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(productDtos, "Products retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductDto>>.FailureResult("Error retrieving products", ex.Message);
        }
    }

    public async Task<ApiResponse<PagedResponse<ProductDto>>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
            var (products, totalCount) = await _unitOfWork.Products.GetPagedAsync(pageNumber, pageSize, cancellationToken);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            var pagedResponse = new PagedResponse<ProductDto>(productDtos, pageNumber, pageSize, totalCount);
            
            return ApiResponse<PagedResponse<ProductDto>>.SuccessResult(pagedResponse, "Products retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PagedResponse<ProductDto>>.FailureResult("Error retrieving products", ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId, cancellationToken);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(productDtos, "Products retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductDto>>.FailureResult("Error retrieving products", ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductDto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm, cancellationToken);
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return ApiResponse<IEnumerable<ProductDto>>.SuccessResult(productDtos, $"Found {productDtos.Count()} products");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductDto>>.FailureResult("Error searching products", ex.Message);
        }
    }

    public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if category exists
            var categoryExists = await _unitOfWork.Categories.ExistsAsync(request.CategoryId, cancellationToken);
            if (!categoryExists)
                return ApiResponse<ProductDto>.FailureResult("Invalid category", $"Category with ID {request.CategoryId} does not exist");

            var product = _mapper.Map<Product>(request);
            var createdProduct = await _unitOfWork.Products.AddAsync(product, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var productDto = _mapper.Map<ProductDto>(createdProduct);
            return ApiResponse<ProductDto>.SuccessResult(productDto, "Product created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.FailureResult("Error creating product", ex.Message);
        }
    }

    public async Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingProduct = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
            if (existingProduct == null)
                return ApiResponse<ProductDto>.FailureResult("Product not found", $"Product with ID {id} does not exist");

            // Check if category exists
            var categoryExists = await _unitOfWork.Categories.ExistsAsync(request.CategoryId, cancellationToken);
            if (!categoryExists)
                return ApiResponse<ProductDto>.FailureResult("Invalid category", $"Category with ID {request.CategoryId} does not exist");

            _mapper.Map(request, existingProduct);
            existingProduct.UpdatedAt = DateTime.UtcNow;
            
            await _unitOfWork.Products.UpdateAsync(existingProduct, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var productDto = _mapper.Map<ProductDto>(existingProduct);
            return ApiResponse<ProductDto>.SuccessResult(productDto, "Product updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductDto>.FailureResult("Error updating product", ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _unitOfWork.Products.ExistsAsync(id, cancellationToken);
            if (!exists)
                return ApiResponse<bool>.FailureResult("Product not found", $"Product with ID {id} does not exist");

            await _unitOfWork.Products.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true, "Product deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResult("Error deleting product", ex.Message);
        }
    }
}
