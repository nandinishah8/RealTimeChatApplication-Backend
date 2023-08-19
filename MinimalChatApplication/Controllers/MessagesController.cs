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
        //[Authorize]
        public async Task<IActionResult> PostMessage(sendMessageRequest message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Message sending failed due to validation errors." });
            }

            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _messageService.PostMessage(message, senderId);

            if (result.Result is OkObjectResult okResult && okResult.Value is sendMessageResponse messageResponse)
            {
                // Notify the sender and receiver using SignalR
                await _hubContext.Clients.User(messageResponse.SenderId).SendAsync("ReceiveMessage", messageResponse);
                await _hubContext.Clients.User(messageResponse.ReceiverId).SendAsync("ReceiveMessage", messageResponse);

                return Ok(messageResponse);
            }
            else if (result.Result is BadRequestObjectResult badRequestResult)
            {
                return BadRequest(badRequestResult.Value);
            }

            return BadRequest(new { error = "An error occurred while sending the message." });
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

            var userId = User.FindFirstValue("sub"); // Get user ID from claims

            var messages = await _messageService.GetConversationHistory(request, userId);

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





        //// PUT: api/Messages/5

        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMessage(int id, Message message)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return new BadRequestObjectResult(new { message = "message editing failed due to validation errors." });
        //    }

        //    return await _messageService.PutMessage(id, message);

        //}


        //// DELETE: api/Message/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteMessage(int id)
        //{
        //    return await _messageService.DeleteMessage(id);
        //}


        //[HttpGet("/api/messages/search/{result}")]

        //public async Task<IActionResult> SearchResult(string result)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(new { message = "message sending failed due to validation errors." });
        //    }

        //    var message = await _messageService.GetMessageHistory(result);

        //    return Ok(message.Select(u => new
        //    {
        //        id = u.Id,
        //        senderId = u.SenderId,
        //        receiverId = u.ReceiverId,
        //        content = u.Content,
        //        timestamp = u.Timestamp
        //    }));
        //}

    }
}



