using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        //private readonly MinimalChatContext _context;

        public UsersController(IUserService userService)
        {
            _userService = userService;

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
    }



    //// GET: api/Users
    //[HttpGet]
    //[Authorize]
    //public async Task<IActionResult> GetUserList()
    //{
    //    try
    //    {
    //        var currentUser = HttpContext.User;
    //        var userId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    //        var users = await _userService.GetUserList(userId);
    //        return Ok(users);
    //    }
    //    catch
    //    {
    //        return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
    //    }
    //}


    //private string GenerateJwtToken(User user)
    //{
    //    var claims = new[]
    //    {
    //            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //            new Claim(ClaimTypes.Name, user.Name),
    //            new Claim(ClaimTypes.Email, user.Email)
    //        };

    //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_here"));
    //    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    //    var token = new JwtSecurityToken(
    //        issuer: "your_issuer_here",
    //        audience: "your_audience_here",
    //        claims: claims,
    //        expires: DateTime.UtcNow.AddMinutes(10),
    //        signingCredentials: signIn);

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}

}
