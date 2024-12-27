using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Repositories.Contracts;
using PaginationResultWebApi.Services.Contracts;

namespace PaginationResultWebApi.Services;

public class GuitarService(IGuitarRepository guitarRepository) : IGuitarService
{
    public async Task<List<Guitar>> ListAll()
    {
        return await guitarRepository.ListAll();
    }

    public async Task<PaginatedList<Guitar>> ListAllByPagination(int pageIndex, int pageSize)
    {
        return await guitarRepository.ListAllByPagination(pageIndex, pageSize);
    }
}