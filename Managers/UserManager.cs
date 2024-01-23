using System.Collections.Generic;
using System.Threading.Tasks;
using SampleMvcApp.Models;
using SampleMvcApp.Repositories;

namespace SampleMvcApp.Managers;

public sealed class UserManager(IUserRepository userRepository) : IUserManager
{
    public Task<User> Create(User user) => userRepository.Create(user);

    public Task<User> GetByName(string username) => userRepository.GetByName(username);

    public Task<IReadOnlyCollection<User>> GetAll() => userRepository.GetAll();

    public Task<User> Update(User user) => userRepository.Update(user);

    public Task Delete(int id) => userRepository.Delete(id);
}