using System.Collections.Generic;
using System.Threading.Tasks;
using SampleMvcApp.Data;
using SampleMvcApp.Models;
using Microsoft.EntityFrameworkCore;

namespace SampleMvcApp.Repositories;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> Create(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetByName(string username) => await context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<IReadOnlyCollection<User>> GetAll() => await context.Users.ToArrayAsync();

    public async Task<User> Update(User user)
    {
        context.Entry(user).State = EntityState.Modified;
        await context.SaveChangesAsync();
        return user;
    }

    public async Task Delete(int id)
    {
        var userToDelete = await context.Users.FindAsync(id);
        if (userToDelete != null)
        {
            context.Users.Remove(userToDelete);
            await context.SaveChangesAsync();
        }
    }
}