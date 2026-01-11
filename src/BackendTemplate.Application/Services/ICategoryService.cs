using BackendTemplate.Application.DTOs;
using BackendTemplate.Shared.Models;

namespace BackendTemplate.Application.Services;

public interface ICategoryService
{
    Task<ApiResponse<CategoryDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
