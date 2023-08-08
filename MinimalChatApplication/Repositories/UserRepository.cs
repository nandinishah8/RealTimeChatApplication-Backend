using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;
using System.Linq.Expressions;

namespace MinimalChatApplication.Repositories
{
    public class UserRepository : IUserRepository
    {
        
        private readonly UserManager<IdentityUser> _userManager;

        public UserRepository(UserManager<IdentityUser> userManager)
        {
           
            _userManager = userManager;
        }

        public Task<IdentityUser> FindByEmailAsync(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }
        public Task<IdentityResult> CreateAsync(IdentityUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }
    }
}


