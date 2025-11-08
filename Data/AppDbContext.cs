using Microsoft.EntityFrameworkCore;
using PaginationResultWebApi.Entities;

namespace PaginationResultWebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Guitar> Guitar { get; set; }
    public DbSet<Customer> Customer { get; set; }

    [DbFunction(name: "SOUNDEX", IsBuiltIn = true)]
    public string FuzzySearch(string query)
    {
        throw new NotImplementedException();
    }
}