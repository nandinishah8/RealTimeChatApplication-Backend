﻿using MinimalChatApplication.Interfaces;
using System;
using MinimalChatApplication.Models;
using MinimalChatApplication.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApplication.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MinimalChatApplication.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly Connection _userConnectionManager;
        private readonly IChannelRepository _channelRepository;


        public MessageService(IMessageRepository messageRepository, IHubContext<ChatHub> hubContext, Connection userConnectionManager, IHttpContextAccessor httpContextAccessor, IChannelRepository channelRepository)
        {
            _messageRepository = messageRepository;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _userConnectionManager = userConnectionManager;
            _channelRepository = channelRepository;

        }

        public async Task<sendMessageResponse> PostMessage(sendMessageRequest model, string senderId)
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
                var receiverConnectionId = await _userConnectionManager.GetConnectionIdAsync(message.ReceiverId);
                Console.WriteLine(receiverConnectionId);

                if (!string.IsNullOrEmpty(receiverConnectionId))
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveOne", message, senderId);
                }


                var response = new sendMessageResponse
                {
                    MessageId = message.Id,
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    Content = message.Content,
                    Timestamp = message.Timestamp
                };

                return response;
            }
            catch (Exception ex)
            {
                return new sendMessageResponse();
            }
        }





        public async Task<List<Message>> GetConversationHistory(ConversationRequest request, string userId)
        {
            return await _messageRepository.GetMessages(userId, request.UserId, request.Count, request.Before);
        }

        private string GetCurrentUserId()
        {
            
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }


        public async Task<IActionResult> PutMessage(int id, EditMessage message)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var messages = await _messageRepository.GetMessageByIdAsync(id);

            if (messages == null)
            {
                return new NotFoundObjectResult(new { message = "message not found" });
            }

            if (userId != messages.SenderId)
            {
                return new UnauthorizedObjectResult(new { message = "Unauthorized access" });
            }

            messages.Content = message.Content;
            await _messageRepository.UpdateMessage(id, message);

            var senderConnectionId = await _userConnectionManager.GetConnectionIdAsync(userId);
            var receiverConnectionId = await _userConnectionManager.GetConnectionIdAsync(messages.ReceiverId);

            if (senderConnectionId != null)
            {
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("ReceiveEdited", messages);
            }

            if (receiverConnectionId != null)
            {
                await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveEdited", messages);
            }

            return new OkObjectResult(new { message = "Message edited successfully" });
        }

        public async Task<IActionResult> DeleteMessage(int id)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var messages = await _messageRepository.GetMessageByIdAsync(id);

            if (messages == null)
            {
                return new NotFoundObjectResult(new { message = "message not found" });
            }

            if (userId != messages.SenderId)
            {
                return new UnauthorizedObjectResult(new { message = "Unauthorized access" });
            }

            await _messageRepository.DeleteMessage(messages);

            var senderConnectionId = await _userConnectionManager.GetConnectionIdAsync(userId);
            var receiverConnectionId = await _userConnectionManager.GetConnectionIdAsync(messages.ReceiverId);

            if (senderConnectionId != null)
            {
                await _hubContext.Clients.Client(senderConnectionId).SendAsync("ReceiveDeleted", messages);
            }

            if (receiverConnectionId != null)
            {
                await _hubContext.Clients.Client(receiverConnectionId).SendAsync("ReceiveDeleted", messages);
            }

            return new OkObjectResult(new { message = "Message deleted successfully" });
        }

        public async Task<List<Message>> GetMessageHistory(string result)
        {
            var getHistory = await _messageRepository.GetMessageHistory(result);

            return getHistory;


        }

        public async Task<Message> SendMessageToChannel(ChannelMessage message, string senderId)
        {
            var channel = await _channelRepository.GetChannelAsync(message.ChannelId);

            if (channel == null)
            {
               
                return null;
            }

            var channelMember = await _channelRepository.GetMembersInChannelAsync(message.ChannelId);

            if (channelMember == null)
            {
                
                return null;
            }


            Message newMessage = null;



            newMessage = new Message
            {
                SenderId = senderId,

                ChannelId = message.ChannelId,
                Content = message.Content,
                Timestamp = DateTime.Now
            };

            await _messageRepository.AddMessageAsync(newMessage);




            return newMessage;
        }

        public async Task<List<Message>> GetChannelMessages(int channelId)
        {

            return await _messageRepository.GetChannelMessages(channelId);
        }


        public async Task<IActionResult> PutChannelMessage(int id, EditMessage message)
        {
         

            var messages = await _messageRepository.GetMessageByIdAsync(id);

            if (messages == null)
            {
                return new NotFoundObjectResult(new { message = "message not found" });
            }

           

            messages.Content = message.Content;
            await _messageRepository.UpdateChannelMessage(id, message);


           

            return new OkObjectResult(new { message = "Message edited successfully" });
        }

        public async Task<bool> DeleteChannelMessage(int messageId, string currentUserId)
        {
            
            var message = await _messageRepository.GetMessageByIdAsync(messageId);

            if (message != null && message.SenderId == currentUserId)
            {
                return await _messageRepository.DeleteMessageAsync(messageId);
            }

            return false;
        }

        public async Task<IActionResult> DeleteChannelMessage(int id,int channelId)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var message = await _messageRepository.GetChannelMessageByIdAsync(channelId);
           
                await _messageRepository.DeleteChannelMessage(message);
            


            return new OkObjectResult(new { message = "Message deleted successfully" });
        }


    }
}

