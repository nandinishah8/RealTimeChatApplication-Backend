using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Data;
using MinimalChatApplication.Models;
using MinimalChatApplication.Services;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MinimalChatApplication.Interfaces;
using Microsoft.AspNetCore.Identity;
using NuGet.Common;
using Azure;
using NuGet.Protocol.Plugins;

namespace MinimalChatApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly MinimalChatContext _context;

        public UsersController(IUserService userService, UserManager<IdentityUser> userManager)
        {
            _userService = userService;
            _userManager = userManager;

        }


        // POST: api/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistration request)
        {
            var (success, message, userDto) = await _userService.RegisterAsync(request);

            if (success)
            {
                return Ok(new { Message = message, User = userDto });
            }
            else
            {
                return BadRequest(new { error = message });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(loginRequest request)
        {
            var (success, message, response) = await _userService.AuthenticateAsync(request);

            if (success)
            {
                return Ok(response);
            }
            else
            {
                return Unauthorized(new { error = message });
            }
        }

        [HttpPost("/api/SocialLogin")]

        public async Task<IActionResult> SocialLogin(tokenRequest token)
        {
           
            Console.WriteLine(token.TokenId);
            var user = await _userService.VerifyGoogleTokenAsync(token.TokenId);

            return Ok(user);
        }


        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetUserList()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Console.Out.WriteLineAsync(currentUserId);

            var users = await _userService.GetUserListAsync(currentUserId);

            if (users == null)
            {
                return NotFound();
            }

            var userListResponse =  users.Select(u => new
            {
                id = u.Id,
                name = u.UserName,
                email = u.Email
            }).ToList();

            return Ok(userListResponse);
        }
    }
  
}

    

