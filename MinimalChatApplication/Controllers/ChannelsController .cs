﻿using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;
using System;
using System.Collections.Generic;
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




        [HttpGet("{id}")]
        public async Task<IActionResult> GetChannel(int id)
        {
            try
            {
                var channel = await _channelService.GetChannelAsync(id);
                if (channel == null)
                {
                    return NotFound(new { message = "Channel not found" });
                }
                return Ok(channel);
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
    }
}
