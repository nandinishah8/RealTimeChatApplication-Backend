using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Data;
using MinimalChatApplication.Models;
using BCrypt.Net;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Repositories;
using Microsoft.AspNetCore.Identity;
using NuGet.Protocol.Plugins;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;

namespace MinimalChatApplication.Services
{
    public class UserService : IUserService
    {

        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, UserManager<IdentityUser> userManager, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool success, string message, RegistrationDto userDto)> RegisterAsync(UserRegistration model)
        {
            // Check if the email is already registered
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return (false, "Email is already registered.", null);
            }

            var newUser = new IdentityUser
            {
                UserName = model.Name,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                // Handle registration error here
                return (false, "Registration failed.", null);
            }

            var userDto = new RegistrationDto
            {

                Name = newUser.UserName,
                Email = newUser.Email,
                Password = newUser.PasswordHash
            };

            return (true, "Registration successful.", userDto);
        }

        public async Task<(bool success, string message, LoginResponse response)> AuthenticateAsync(loginRequest loginData)
        {
            var user = await _userManager.FindByEmailAsync(loginData.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginData.Password))
            {
                var token = GenerateJwtToken(user.Id, user.UserName, user.Email);
                var userProfile = new UserProfile
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email
                };

                var response = new LoginResponse
                {

                    Profile = userProfile,
                    Token = token,
                };

                return (true, "Authentication successful.", response);
            }

            return (false, "Authentication failed.", null);
        }

        public async Task<LoginResponse> VerifyGoogleTokenAsync(string tokenId)
        {
            try
            {
                Console.WriteLine("Validating Google Token...");
                var validPayload = await GoogleJsonWebSignature.ValidateAsync(tokenId);
                Console.WriteLine($"Valid Google Token. Email: {validPayload.Email}");
                var userByEmail = await _userManager.FindByEmailAsync(validPayload.Email);

                if (validPayload.EmailVerified)
                {
                    var user = await _userManager.FindByLoginAsync("Google", validPayload.Subject);

                    if (user == null)
                    {
                        user = await _userManager.FindByEmailAsync(validPayload.Email);

                        if (userByEmail == null)
                        {
                            userByEmail = new IdentityUser
                            {
                                UserName = validPayload.GivenName,
                                Email = validPayload.Email,
                            };


                            var userLoginInfo = new UserLoginInfo("Goggle", validPayload.Subject, "Goggle");
                            var result = await _userManager.CreateAsync(userByEmail);

                            if (result.Succeeded)
                            {
                                await _userManager.AddLoginAsync(userByEmail, userLoginInfo);
                            }
                        }
                    }

                    var users = user ?? userByEmail;

                    if (users == null)
                    {
                        return null;
                    }

                    var generatedToken = GenerateJwtToken(user.Id, user.UserName, user.Email);

                    var loginResponse = new LoginResponse
                    {
                        Token = generatedToken,
                        Profile = new UserProfile
                        {
                            Id = users.Id,
                            Name = users.UserName,
                            Email = users.Email
                        }
                    };

                    return loginResponse;
                }

                return null;

            }
            catch (InvalidJwtException ex)
            {
                // The token is invalid. Handle the error.
                throw new Exception("Invalid token: " + ex.Message);
            }
        }


        private string GenerateJwtToken(string id, string name, string email)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier,id),
                    new Claim(ClaimTypes.Name,name),
                    new Claim(ClaimTypes.Email,email)
                   
                 };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(30), // Token expiration time
                    signingCredentials: signIn);

                return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public async Task<List<IdentityUser>> GetUserListAsync(string currentUserId)
        {

            var users = await _userManager.Users
             .Where(u => u.Id != currentUserId)
             .ToListAsync();

            return users;
        }





    }

}



