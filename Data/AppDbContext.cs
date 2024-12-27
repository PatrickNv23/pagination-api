using Microsoft.EntityFrameworkCore;
using PaginationResultWebApi.Entities;

namespace PaginationResultWebApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Guitar> Guitar { get; set; }
}