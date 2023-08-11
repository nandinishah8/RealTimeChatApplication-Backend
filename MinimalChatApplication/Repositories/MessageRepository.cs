using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MinimalChatContext _dbContext;

        public MessageRepository(MinimalChatContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ActionResult SendMessage(int senderId, int receiverId, string content, out int messageId)
        {
            
            messageId = 0; // Initialize messageId

            try
            {
                var message = new sendMessageResponse
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Content = content,
                    Timestamp = DateTime.UtcNow
                };

                _dbContext.Messages.Add(message);
                _dbContext.SaveChanges();

                messageId = message.MessageId; // Set the generated message ID
               
            }
            catch (Exception)
            {
                // Handle exceptions and logging as needed
                return false;
            }
        }
    }
}
