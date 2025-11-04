using Taller.Backend.Repositories.Interfaces;
using Taller.Backend.UnitOfWork.Implementations;
using Taller.Backend.UnitsOfWork.Interfaces;
using Taller.Shared.DTOs;
using Taller.Shared.Entities;
using Taller.Shared.Responses;

namespace Taller.Backend.UnitsOfWork.Implementations;

public class CategoriesUnitOfWork : GenericUnitOfWork<Category>, ICategoriesUnitOfWork
{
    private readonly ICategoriesRepository _categoriesRepository;

    public CategoriesUnitOfWork(IGenericRepository<Category> repository, ICategoriesRepository categoriesRepository) : base(repository)
    {
        _categoriesRepository = categoriesRepository;
    }

    public async Task<IEnumerable<Category>> GetComboAsync() => await _categoriesRepository.GetComboAsync();

    public override async Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination) => await _categoriesRepository.GetAsync(pagination);

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _categoriesRepository.GetTotalRecordsAsync(pagination);
}