using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MinimalChatContext _dbcontext;
        private readonly UserManager<IdentityUser> _userManager;

        public MessageRepository(MinimalChatContext context, UserManager<IdentityUser> userManager)
        {
            _dbcontext = context;
            _userManager = userManager;
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            _dbcontext.Messages.Add(message);
            Console.WriteLine(message);
            await _dbcontext.SaveChangesAsync();
            return message;
        }



        public async Task DeleteMessage(Message message)
        {
            _dbcontext.Remove(message);
            await _dbcontext.SaveChangesAsync();
        }



        public async Task<List<Message>> GetMessages(string userId, string otherUserId, int count, DateTime? before)
        {

            var sentMessages = _dbcontext.Messages
        .Where(m => m.SenderId == userId && m.ReceiverId == otherUserId);

            var receivedMessages = _dbcontext.Messages
                .Where(m => m.SenderId == otherUserId && m.ReceiverId == userId);

            var combinedMessages = sentMessages.Concat(receivedMessages);

            if (before.HasValue)
            {
                combinedMessages = combinedMessages.Where(m => m.Timestamp < before);
            }

            var messages = await combinedMessages
                .OrderByDescending(m => m.Timestamp)
                .Take(count)
                .ToListAsync();

            return messages;
        }


        public async Task<Message> GetMessageByIdAsync(int id)
        {
            return await _dbcontext.Messages.FindAsync(id);
        }


        public async Task UpdateMessage(int messageId, EditMessage editMessage)
        {
            var message = await _dbcontext.Messages.FindAsync(messageId);

            if (message == null)
            {
                // Handle the case where the message is not found
                return;
            }

            message.Content = editMessage.Content;
            // Update other properties as needed

            await _dbcontext.SaveChangesAsync();
        }

        public async Task<List<Message>> GetMessageHistory(string result)
        {
            return await _dbcontext.Messages.Where(u => u.Content.Contains(result)).ToListAsync();

        }

        public async Task<List<Message>> GetChannelMessages(int channelId, int count)
        {
            return await _dbcontext.Messages
                .Where(m => m.ChannelId == channelId)
                .OrderByDescending(m => m.Timestamp)
                .Take(count)
                .ToListAsync();
        }

    }
}

