using BackendTemplate.Application.DTOs;
using BackendTemplate.Application.Services;
using BackendTemplate.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendTemplate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await _productService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResponse<ProductDto>>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetPagedAsync(pageNumber, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _productService.GetByIdAsync(id, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetByCategory(
        int categoryId,
        CancellationToken cancellationToken)
    {
        var result = await _productService.GetByCategoryAsync(categoryId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> Search(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var result = await _productService.SearchAsync(searchTerm, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _productService.CreateAsync(request, cancellationToken);
        
        if (!result.Success)
            return BadRequest(result);
        
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(
        int id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _productService.UpdateAsync(id, request, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _productService.DeleteAsync(id, cancellationToken);
        
        if (!result.Success)
            return NotFound(result);
        
        return Ok(result);
    }
}
