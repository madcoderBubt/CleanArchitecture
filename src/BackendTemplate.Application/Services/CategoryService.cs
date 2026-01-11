using AutoMapper;
using BackendTemplate.Application.DTOs;
using BackendTemplate.Domain.Entities;
using BackendTemplate.Domain.Interfaces;
using BackendTemplate.Shared.Models;

namespace BackendTemplate.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CategoryDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            
            if (category == null)
                return ApiResponse<CategoryDto>.FailureResult("Category not found", $"Category with ID {id} does not exist");

            var categoryDto = _mapper.Map<CategoryDto>(category);
            return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Category retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.FailureResult("Error retrieving category", ex.Message);
        }
    }

    public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _unitOfWork.Categories.GetCategoriesWithProductCountAsync(cancellationToken);
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return ApiResponse<IEnumerable<CategoryDto>>.SuccessResult(categoryDtos, "Categories retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<CategoryDto>>.FailureResult("Error retrieving categories", ex.Message);
        }
    }

    public async Task<ApiResponse<CategoryDto>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = _mapper.Map<Category>(request);
            var createdCategory = await _unitOfWork.Categories.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var categoryDto = _mapper.Map<CategoryDto>(createdCategory);
            return ApiResponse<CategoryDto>.SuccessResult(categoryDto, "Category created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<CategoryDto>.FailureResult("Error creating category", ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var category = await _unitOfWork.Categories.GetCategoryWithProductsAsync(id, cancellationToken);
            
            if (category == null)
                return ApiResponse<bool>.FailureResult("Category not found", $"Category with ID {id} does not exist");

            if (category.Products.Any())
                return ApiResponse<bool>.FailureResult("Cannot delete category", "Category has associated products");

            await _unitOfWork.Categories.DeleteAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.SuccessResult(true, "Category deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.FailureResult("Error deleting category", ex.Message);
        }
    }
}
