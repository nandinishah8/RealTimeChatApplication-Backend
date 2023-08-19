using MinimalChatApplication.Interfaces;
using System;
using MinimalChatApplication.Models;
using MinimalChatApplication.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApplication.Hubs;
using Microsoft.AspNetCore.Mvc;

namespace MinimalChatApplication.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHubContext<ChatHub> _hubContext;
       

        public MessageService(IMessageRepository messageRepository, IHubContext<ChatHub> hubContext)
        {
            _messageRepository = messageRepository;
            _hubContext = hubContext;
           
        }

        public async Task<ActionResult<sendMessageResponse>> PostMessage(sendMessageRequest model, string senderId)
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = model.ReceiverId,
                Content = model.Content,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                await _messageRepository.AddMessageAsync(message);

                var response = new sendMessageResponse
                {
                    MessageId = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content,
                    Timestamp = message.Timestamp
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { error = $"Message sending failed: {ex.Message}" });
            }
        }



        public async Task<List<Message>> GetMessageHistory(string result)
        {
            var getHistory = await _messageRepository.GetMessageHistory(result);

            return getHistory;


        }

        public async Task<List<Message>> GetConversationHistory(ConversationRequest request, string userId)
        {
            return await _messageRepository.GetMessages(userId, request.UserId, request.Count, request.Before);
        }


        //public async Task<IActionResult> PutMessage(int id, Message message)
        //{
        //    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var messages = await _messageRepository.GetMessageById(id);

        //    if (messages == null)
        //    {
        //        return new NotFoundObjectResult(new { message = "message not found" });
        //    }

        //    if (userId != messages.SenderId)
        //    {
        //        return new UnauthorizedObjectResult(new { message = "Unauthorized access" });
        //    }

        //    messages.Content = message.Content;
        //    await _messageRepository.UpdateMessage(messages);

        //    var senderConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);
        //    var receiverConnectionId = await _userConnectionService.GetConnectionIdAsync(messages.ReceiverId);

        //    if (senderConnectionId != null)
        //    {
        //        await _hubContext.Clients.Client(senderConnectionId).SendAsync("ReceiveEdited", messages);
        //    }

        //    if (receiverConnectionId != null)
        //    {
        //        await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveEdited", messages);
        //    }

        //    return new OkObjectResult(new { message = "Message edited successfully" });
        //}

        //public async Task<IActionResult> DeleteMessage(int id)
        //{
        //    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    var messages = await _messageRepository.GetMessageById(id);

        //    if (messages == null)
        //    {
        //        return new NotFoundObjectResult(new { message = "message not found" });
        //    }

        //    if (userId != messages.SenderId)
        //    {
        //        return new UnauthorizedObjectResult(new { message = "Unauthorized access" });
        //    }

        //    await _messageRepository.DeleteMessage(messages);

        //    var senderConnectionId = await _userConnectionService.GetConnectionIdAsync(userId);
        //    var receiverConnectionId = await _userConnectionService.GetConnectionIdAsync(messages.ReceiverId);

        //    if (senderConnectionId != null)
        //    {
        //        await _hubContext.Clients.Client(senderConnectionId).SendAsync("ReceiveDeleted", messages);
        //    }

        //    if (receiverConnectionId != null)
        //    {
        //        await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveDeleted", messages);
        //    }

        //    return new OkObjectResult(new { message = "Message deleted successfully" });
        //}



    }
}

