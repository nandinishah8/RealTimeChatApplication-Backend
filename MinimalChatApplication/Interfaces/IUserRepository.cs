using Microsoft.AspNetCore.Identity;
using MinimalChatApplication.Models;
using System.Linq.Expressions;

namespace MinimalChatApplication.Interfaces
{

        public interface IUserRepository
        {
        //Task<User> Register(User model);
        Task<IdentityUser> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(IdentityUser user, string password);
        //Task<User> Login(loginRequest loginData);
        //Task<IEnumerable<User>> GetUserList(int currentUserId);
    }

}
