using Microsoft.EntityFrameworkCore;
using PaginationResultWebApi.Data;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Repositories.Contracts;

namespace PaginationResultWebApi.Repositories;

public class GuitarRepository(AppDbContext context) : IGuitarRepository
{
    public async Task<List<Guitar>> ListAll()
    {
        return await context.Guitar.ToListAsync();
    }

    public async Task<PaginatedList<Guitar>> ListAllByPagination(int pageIndex, int pageSize)
    {
        var guitars = await context.Guitar
            .OrderBy(b => b.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var count = await context.Guitar.CountAsync();
        var totalPages = (int)Math.Ceiling((double)count / pageSize);
        return new PaginatedList<Guitar>(guitars, pageIndex, totalPages);
    }
}