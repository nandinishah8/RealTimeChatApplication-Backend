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

        public MessagesController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
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



        [HttpPost("mark-all-as-read/{receiverId}")]
        public IActionResult MarkAllMessagesAsRead(string receiverId)
        {
            try
            {
                _messageService.MarkAllMessagesAsRead(receiverId);
                //return Ok("All messages marked as read.");
                return Ok(new { message = "All messages marked as read." });
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log the error and return an error response
                return StatusCode(500, "An error occurred while marking messages as read.");
            }
        }



        [HttpGet("unread-counts/{userId}")]
        public IActionResult GetReadUnreadMessageCounts(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { message = "Invalid user ID." });
            }

            Dictionary<string, int> readUnreadCounts = _messageService.GetReadUnreadMessageCounts(userId);

            if (readUnreadCounts == null)
            {
                return NotFound(new { message = "User not found or counts could not be retrieved." });
            }

            return Ok(readUnreadCounts);
        }
    }
}




