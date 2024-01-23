using System.Collections.Generic;
using System.Threading.Tasks;
using SampleMvcApp.Models;

namespace SampleMvcApp.Managers;

public interface IUserManager
{
    Task<User> Create(User user);
    Task<User> GetByName(string username);
    Task<IReadOnlyCollection<User>> GetAll();
    Task<User> Update(User user);
    Task Delete(int id);
}