using System.Linq.Expressions;
using PaginationResultWebApi.Entities;

namespace PaginationResultWebApi.Repositories.Contracts;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> ListAll();
    Task<T> AddAsync(T entity);
    Task<T> FindOneAsync(Expression<Func<T, bool>> predicate);
    Task<PaginatedList<T>> ListAllByPagination(int pageIndex, int pageSize, Expression<Func<T, object>> orderExpression);
}