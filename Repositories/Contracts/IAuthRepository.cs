using System.Linq.Expressions;
using PaginationResultWebApi.Entities;

namespace PaginationResultWebApi.Repositories.Contracts;

public interface IAuthRepository
{
    Task<Customer> FindOneAsync(Expression<Func<Customer, bool>> predicate);
    Task AddAsync(Customer customer);
}