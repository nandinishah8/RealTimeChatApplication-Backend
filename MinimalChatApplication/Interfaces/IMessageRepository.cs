﻿using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IMessageRepository
    {

        Task<Message> AddMessageAsync(Message message);
        Task<List<Message>> GetMessages(string userId, string otherUserId, int count, DateTime? before);

        Task<Message> GetMessageByIdAsync(int id);

        Task<List<Message>> GetMessageHistory(string result);

        Task UpdateMessage(Message message);

        Task DeleteMessage(Message message);

    }
}
