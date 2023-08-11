using MinimalChatApplication.Interfaces;
using System;
using MinimalChatApplication.Models;
using MinimalChatApplication.Repositories;

namespace MinimalChatApplication.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public sendMessageResponse SendMessage(sendMessageRequest request)
        {
            var response = new sendMessageResponse();

            if (string.IsNullOrEmpty(request.Content))
            {
                // You can handle validation and return appropriate response without ErrorMessage
                response.MessageId = -1; // Set an invalid ID to indicate a failure
                return response;
            }

            if (_messageRepository.SendMessage(0,request.ReceiverId, request.Content, out int messageId))
            {
                response.MessageId = messageId;
                response.SenderId = 0;
                response.ReceiverId = request.ReceiverId;
                response.Content = request.Content;
                response.Timestamp = DateTime.UtcNow;
            }
            else
            {
                response.MessageId = -1; // Set an invalid ID to indicate a failure
            }

            return response;
        }
   

        private int GenerateRandomMessageId()
        {
            // Implement your logic to generate a random message ID
            return new Random().Next(1000, 9999);
        }
    }
}

