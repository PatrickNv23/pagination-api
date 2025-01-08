using PaginationResultWebApi.Entities;

namespace PaginationResultWebApi.Services.Contracts;

public interface IGuitarService
{
    Task<List<Guitar>> ListAll();
    Task<PaginatedList<Guitar>> ListAllByPagination(int pageIndex, int pageSize);
    Task<Guitar> Add(Guitar guitar);
}