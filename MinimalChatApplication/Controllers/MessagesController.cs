using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Data;
using MinimalChatApplication.Hubs;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;
using MinimalChatApplication.Services;


namespace MinimalChatApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IChannelService _channelService;

        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext, IChannelService channelService)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _channelService = channelService;
        }

        // POST: api/Messages

        [HttpPost]
        public async Task<IActionResult> PostMessage(sendMessageRequest message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Message sending failed due to validation errors." });
            }

            var currentUser = HttpContext.User;
            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            sendMessageResponse messageResponse = await _messageService.PostMessage(message, userId);
            Console.WriteLine(userId);



            return Ok(messageResponse);
        }




        //GET: api/Message
        [HttpGet]

        public async Task<IActionResult> GetConversationHistory([FromQuery] ConversationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid request parameter." });
            }

            Console.WriteLine(request);
            var currentUser = HttpContext.User;
            string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Console.WriteLine(userId);

            List<Message> messages = await _messageService.GetConversationHistory(request, userId);

            if (messages == null)
            {
                return NotFound(new { message = "User or conversation not found" });
            }

            var response = new ConversationHistoryResponseDto
            {
                Messages = messages.Select(m => new ConversationResponse
                {
                    Id = m.Id,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    Timestamp = m.Timestamp
                })
            };

            return Ok(response);
        }

        // PUT: api/Messages/5

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMessage(int id, EditMessage message)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(new { message = "message editing failed due to validation errors." });
            }

            return await _messageService.PutMessage(id, message);

        }


        // DELETE: api/Message/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            return await _messageService.DeleteMessage(id);
        }


        [HttpGet("/api/messages/search/{result}")]

        public async Task<IActionResult> SearchResult(string result)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "message sending failed due to validation errors." });
            }

            var message = await _messageService.GetMessageHistory(result);

            return Ok(message.Select(u => new
            {
                id = u.Id,
                senderId = u.SenderId,
                receiverId = u.ReceiverId,
                content = u.Content,
                timestamp = u.Timestamp
            }));
        }

        [HttpPost("Channels/messages")]
        [Authorize] 
        public async Task<IActionResult> PostChannelMessage(ChannelMessage message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Message sending failed due to validation errors." });
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var sentMessage = await _messageService.SendMessageToChannel(message, currentUserId);

            // Broadcast the message to all channel members using SignalR
            var channelMembers = await _channelService.GetMembersInChannelAsync(message.ChannelId);
            foreach (var member in channelMembers)
            {
                // Send the message to each member through SignalR
                await _hubContext.Clients.User(member.Id).SendAsync("ReceiveChannelMessage", sentMessage);
            }

            return Ok(sentMessage);
        }
    }






}





