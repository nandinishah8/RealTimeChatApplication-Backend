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

        public UserService(IUserRepository userRepository, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _configuration = configuration;
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
                var token = GenerateJwtToken(user);
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
                var user = await _userRepository.FindByEmailAsync(validPayload.Email);

                if (user == null)
                {
                    //Create a new IdentityUser if not found in the repository
                    var newUser = new IdentityUser
                    {
                        UserName = validPayload.GivenName,
                        Email = validPayload.Email
                    };
                    var result = await _userManager.CreateAsync(newUser);

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error creating user: {error.Description}");
                        }
                        return null;
                    }

                    user = newUser;
                }

                // Generate or retrieve the authentication token
                var token = GenerateJwtToken(user); // Replace with your token generation logic

                var userProfile = new UserProfile
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email  
                };

                var loginResponse = new LoginResponse
                {
                    Token = token,
                    Profile = userProfile
                };

                Console.WriteLine("Login response generated successfully.");
                return loginResponse;
            }
            catch (InvalidJwtException)
            {
                // Token validation failed
                Console.WriteLine("Token validation failed.");
                return null;
            }
        }
    




            private string GenerateJwtToken(IdentityUser user)
            {
                var claims = new[]
                {
                     new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                      new Claim(ClaimTypes.Email, user.Email)
                    // Add additional claims if needed
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


    }

}



