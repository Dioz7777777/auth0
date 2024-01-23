using System;
using Microsoft.EntityFrameworkCore;
using SampleMvcApp.Models;

namespace SampleMvcApp.Data;

public sealed class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
    {
        CreateDatabase();
    }
    
    private void CreateDatabase()
    {
        try
        {
            Database.EnsureCreated();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}