using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PaginationResultWebApi.Data;
using PaginationResultWebApi.Entities;
using PaginationResultWebApi.Repositories.Contracts;

namespace PaginationResultWebApi.Repositories;

public class AuthRepository(AppDbContext context) : IAuthRepository
{
    public async Task<Customer> FindOneAsync(Expression<Func<Customer, bool>> predicate)
    {
        return (await context.Customer.FirstOrDefaultAsync(predicate))!;
    }

    public async Task AddAsync(Customer customer)
    {
        await context.Customer.AddAsync(customer);
        await context.SaveChangesAsync();
    }
}