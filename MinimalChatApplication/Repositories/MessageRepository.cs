using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MinimalChatContext _context;

        public MessageRepository(MinimalChatContext context)
        {
            _context = context;
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }



        public async Task DeleteMessage(Message message)
        {
            _context.Remove(message);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Message>> GetMessageHistory(string result)
        {
            return await  _context.Messages.Where(u => u.Content.Contains(result)).ToListAsync();

        }

        public async Task<List<Message>> GetMessages(string userId, string otherUserId, int count, DateTime? before)
        {
            var query = _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == otherUserId) || (m.SenderId == otherUserId && m.ReceiverId == userId));

            if (before.HasValue)
            {
                query = query.Where(m => m.Timestamp < before);
            }

            var messages = await query.OrderByDescending(m => m.Timestamp)
                                      .Take(count)
                                      .ToListAsync();

            return messages;
        }
    

    public async Task<Message> GetMessageById(int id)
        {
            return await  _context.Messages.FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task UpdateMessage(Message message)
        {
            _context.Messages.Update(message);
            await _context.SaveChangesAsync();
        }
    }
}
