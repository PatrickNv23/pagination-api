using PaginationResultWebApi.Entities;

namespace PaginationResultWebApi.Repositories.Contracts;

public interface IGuitarRepository
{
    Task<List<Guitar>> ListAll();
    Task<PaginatedList<Guitar>> ListAllByPagination(int pageIndex, int pageSize);
    
}