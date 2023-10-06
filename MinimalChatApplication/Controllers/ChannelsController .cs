using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MinimalChatApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly IChannelService _channelService;

        public ChannelsController(IChannelService channelService)
        {
            _channelService = channelService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateChannel(CreateChannelRequest channelRequest)
        {
            // Validate and create a new channel
            var channel = new Channels
            {
                Name = channelRequest.Name,
                Description = channelRequest.Description,
                CreatedAt = DateTime.Now 
                                         
            };

            try
            {
                var createdChannel = await _channelService.CreateChannelAsync(channel);
                return Ok(createdChannel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetChannels()
        {
            try
            {
                var channels = _channelService.GetChannels(); 
                return Ok(channels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching channels.");
            }
        }





        [HttpGet("UserId")]
        public async Task<IActionResult> GetChannelsByUser()
        {
            var currentUser = HttpContext.User;
            string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(userId);

            try
            {
                // Fetch the channels where the specified user is a member
                var channels = await _channelService.GetChannelsByUserAsync(userId);

                return Ok(channels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching channels." });
            }
        }

        [HttpPost("addMembers")]
        public async Task<IActionResult> AddMembersToChannel(AddMembersRequest addMembersRequest)
        {
            try
            {
                var result = await _channelService.AddMembersToChannelAsync(addMembersRequest.ChannelId, addMembersRequest.UserIds);
                if (result)
                {
                    return Ok(new { message = "Members added to the channel successfully" });
                }
                return BadRequest("Failed to add members to the channel");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("members/{channelId}")]
        public async Task<IActionResult> GetMembersInChannel(int channelId)

        {
            try
            {
                var members = await _channelService.GetMembersInChannelAsync(channelId);
                return Ok(members);
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "An error occurred while retrieving members in the channel.");
            }
        }
    }
}

