using Azure.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Migrations;
using MinimalChatApplication.Models;
using System;
//using NuGet.Protocol.Plugins;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MinimalChatApplication.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Connection _userConnectionManager;
        private readonly IMessageService _messageService;
        private Dictionary<string, string> userConnectionMap = new Dictionary<string, string>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        int unreadMessageCount = 0;
        public IHubContext<ChatHub> HubContext { get; }

        public ChatHub(IMessageService messageService, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHub> hubContext, Connection userConnectionManager)
        {
            _messageService = messageService;

            _httpContextAccessor = httpContextAccessor;
            HubContext = hubContext;
            _userConnectionManager = userConnectionManager;
        }


       

        public override async Task OnConnectedAsync()
        {

            var userId = GetCurrentUserId();
            var connectionId = Context.ConnectionId;
            Console.WriteLine($"CID: {connectionId}, UID: {userId}");

            // Associate user ID with connection ID
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(connectionId))
            {
                _userConnectionManager.AddConnection(userId, connectionId);
                await Groups.AddToGroupAsync(connectionId, userId);

            }

            await base.OnConnectedAsync();
        }

        private string GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }

        public void AddConnection(string userId, string connectionId)
        {
            userConnectionMap[userId] = connectionId;
        }

        private async Task<string> GetConnectionId(string userId)
        {
            return await _userConnectionManager.GetConnectionIdAsync(userId);
        }


     

        public async Task SendMessage(ConversationResponse message, string senderId)
        {
            var currentTime = DateTime.UtcNow;
            string userId = GetCurrentUserId();
            var receiverId = message.ReceiverId;
            Console.WriteLine($"ReceiverId: {receiverId}");
            unreadMessageCount++;


            var connectionId = await _userConnectionManager.GetConnectionIdAsync(message.ReceiverId);
           
            await Clients.All.SendAsync("ReceiveOne", message, senderId);
            await Clients.All.SendAsync("UpdateUnreadCount", unreadMessageCount);
            Console.WriteLine(unreadMessageCount);


        }


      public async Task SendEditedMessage(EditMessage editMessage)
      {
           

            await Clients.All.SendAsync("ReceiveEdited", editMessage);
                Console.WriteLine(editMessage.Content);
      
      }


        public async Task SendDeletedMessage(int messageId)
        {
            await Clients.All.SendAsync("ReceiveDeleted", messageId);
        }


        //public async Task MarkMessageAsSeen(string senderConnectionId)
        //{
            
        //    await Clients.All.SendAsync("messageSeen", senderConnectionId);
        //}

        public async Task MarkAllMessagesAsRead(string receiverId)
        {
            
            await Clients.All.SendAsync("AllMessagesRead", receiverId);
        }


    }
}

