using BackendTemplate.Domain.Entities;

namespace BackendTemplate.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetCategoryWithProductsAsync(int categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync(CancellationToken cancellationToken = default);
}
