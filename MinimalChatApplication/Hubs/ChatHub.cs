using Azure.Messaging;
using Microsoft.AspNetCore.SignalR;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;
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
        public IHubContext<ChatHub> HubContext { get; }

        public ChatHub(IMessageService messageService, IHttpContextAccessor httpContextAccessor, IHubContext<ChatHub> hubContext, Connection userConnectionManager)
        {
            _messageService = messageService;

            _httpContextAccessor = httpContextAccessor;
            HubContext = hubContext;
            _userConnectionManager = userConnectionManager;
        }


        //private string GetUserIdFromContext()
        //{
        //    var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    return userId;
        //}

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


        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var userId = GetUserIdFromContext();
        //    var connectionId = Context.ConnectionId;

        //    _userConnectionMap.RemoveConnection(userId, connectionId);

        //    await base.OnDisconnectedAsync(exception);
        //}

        public async Task SendMessage(ConversationResponse message, string senderId)
        {
            string userId = GetCurrentUserId();
            var receiverId = message.ReceiverId;
            Console.WriteLine($"ReceiverId: {receiverId}");


            // var connectionIds = _userConnectionManager.GetConnectionIdAsync(message.ReceiverId);
            //var newmessageResponse = await _messageService.PostMessage(message, senderId);
            // await Clients.Client(connectionId).SendAsync("ReceiveOne", newmessageResponse);
            await Clients.All.SendAsync("ReceiveOne", message, senderId);
            //await Clients.User(senderId).SendAsync("ReceiveOne", message, senderId);
            //await Clients.User(receiverId).SendAsync("ReceiveOne", message, senderId);

        }
    }
}

