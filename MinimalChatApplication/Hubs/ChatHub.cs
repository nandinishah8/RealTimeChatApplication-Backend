﻿using Azure.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Migrations;
using MinimalChatApplication.Models;

using System;
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



            var connectionId = await _userConnectionManager.GetConnectionIdAsync(message.ReceiverId);

            await Clients.All.SendAsync("ReceiveOne", message, senderId);




        }


        public async Task SendEditedMessage(EditMessage editMessage)
        {


            await Clients.All.SendAsync("ReceiveEdited", editMessage);


        }


        public async Task SendDeletedMessage(int messageId)
        {
            await Clients.All.SendAsync("ReceiveDeleted", messageId);
        }

        public async Task CreateChannel(Channels channel)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await Clients.All.SendAsync("ReceiveChannel", channel);
        }

        public async Task UpdateChannel(Channels updatedChannel)
        {

            await Clients.All.SendAsync("ReceiveUpdatedChannel", updatedChannel);
        }

        public async Task EditChannel(int channelId, Channels updatedChannel)
        {

            await Clients.All.SendAsync("ReceiveUpdatedChannel", channelId, updatedChannel);
            await Clients.Group(channelId.ToString()).SendAsync("ReceiveUpdatedChannel", updatedChannel);
        }

        public async Task DeleteChannel(int channelId)
        {

            await Clients.All.SendAsync("ReceiveDeletedChannel", channelId);
            await Clients.Group(channelId.ToString()).SendAsync("ReceiveDeletedChannel", channelId);
        }



        public async Task SendChannelMessage(ChannelMessage message)
        {
            await Clients.All.SendAsync("ReceiveChannelMessage", message);
            var channelId = message.ChannelId.ToString();
            await Clients.Group(channelId).SendAsync("ReceiveChannelMessage", message);
        }

        public async Task GetMessages(int channelId)
        {
           
            List<Message> messages = await _messageService.GetChannelMessages(channelId);

          
            await Clients.Caller.SendAsync("ReceiveMessages", messages);
        }


        public async Task EditChannelMessage(EditMessage editMessage)
        {


            await Clients.All.SendAsync("ReceiveChannelEdited", editMessage);
            

        }
        public async Task DeleteChannelMessage(int messageId)
        {
          
            await Clients.All.SendAsync("ReceiveDeletedChannelMessage", messageId);
            await Clients.Group("channel").SendAsync("ReceiveDeletedChannelMessage", messageId);
        }


    }
}

