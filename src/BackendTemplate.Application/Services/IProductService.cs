using BackendTemplate.Application.DTOs;
using BackendTemplate.Shared.Models;

namespace BackendTemplate.Application.Services;

public interface IProductService
{
    Task<ApiResponse<ProductDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<PagedResponse<ProductDto>>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<ProductDto>>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<ProductDto>> UpdateAsync(int id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
