using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApplication.Hubs;
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
        private readonly IHubContext<ChatHub> _chatHubContext;


        public ChannelsController(IChannelService channelService, IHubContext<ChatHub> chatHubContext)
        {
            _channelService = channelService;
            _chatHubContext = chatHubContext;
        }


        

        [HttpPost]
        public async Task<IActionResult> CreateChannel(CreateChannelRequest channelRequest)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var createdChannel = await _channelService.CreateChannelAsync(channelRequest.Name, channelRequest.Description, userId);
             
                return Ok(createdChannel);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{channelId}")]
        public async Task<IActionResult> EditChannel(int channelId, CreateChannelRequest editChannelRequest)
        {
            try
            {
                var updatedChannel = await _channelService.EditChannelAsync(channelId, editChannelRequest);
                return Ok(updatedChannel);
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

        [HttpDelete("{channelId}")]
        public async Task<IActionResult> DeleteChannel(int channelId)
        {
            try
            {
                bool deleted = await _channelService.DeleteChannelAsync(channelId);
               
                if (deleted)
                {
                    return Ok(new { message = "Channel deleted successfully" });
                }
                return BadRequest("Failed to delete the channel");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [HttpDelete("{channelId}/members")]
        public async Task<IActionResult> DeleteMembersFromChannel(int channelId, List<string> memberIds)
        {
            try
            {
                bool deleted = await _channelService.DeleteMembersFromChannelAsync(channelId, memberIds);
                if (deleted)
                {
                    return Ok(new { message = "Members deleted from the channel successfully" });
                }
                return BadRequest("Failed to delete members from the channel");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}

