using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PaginationResultWebApi.Data;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Repositories.Contracts;

namespace PaginationResultWebApi.Repositories;

public class GenericRepository<T>(AppDbContext context) : IGenericRepository<T> where T : class
{
    public async Task<List<T>> ListAll()
    {
        return await context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        var newEntity = await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
        return newEntity.Entity;
    }

    public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicate)
    {
        return (await context.Set<T>().AsNoTracking().FirstOrDefaultAsync(predicate))!;
    }

    public async Task<PaginatedList<T>> ListAllByPagination(int pageIndex, int pageSize, Expression<Func<T, object>> orderExpression)
    {
        var entities = await context.Set<T>()
            .OrderBy(orderExpression)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var count = await context.Guitar.CountAsync();
        var totalPages = (int)Math.Ceiling((double)count / pageSize);
        return new PaginatedList<T>(entities, pageIndex, totalPages);
    }
}