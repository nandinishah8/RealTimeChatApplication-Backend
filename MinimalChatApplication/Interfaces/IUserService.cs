using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Models;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;


namespace MinimalChatApplication.Interfaces
{
    public interface IUserService
    {
        //Task<User> Register(User model);
        //Task<User> Login(loginRequest loginData);
        //Task<IEnumerable<User>> GetUserList(int currentUserId);

        Task<(bool success, string message, RegistrationDto userDto)> RegisterAsync(UserRegistration model);
        Task<(bool success, string message, LoginResponse response)> AuthenticateAsync(loginRequest loginData);
        //Task<IdentityUser> AuthenticateAsync(loginRequest loginData);
        //Task<IdentityUser> GetUserByIdAsync(string userId);
    }
}
