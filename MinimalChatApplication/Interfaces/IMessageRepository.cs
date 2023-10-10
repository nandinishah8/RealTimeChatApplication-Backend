using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IMessageRepository
    {

        Task<Message> AddMessageAsync(Message message);
        Task<List<Message>> GetMessages(string userId, string otherUserId, int count, DateTime? before);
        Task<Message> GetMessageByIdAsync(int id);
        Task<List<Message>> GetMessageHistory(string result);
        Task UpdateMessage(int messageId, EditMessage editMessage);
        Task DeleteMessage(Message message);

        Task<List<Message>> GetChannelMessages(int channelId, int count);

        //Task<Message> AddChannelMessageAsync(ChannelMessage message);
        //Task<List<ChannelMessage>> GetChannelMessages(int channelId, int count, DateTime? before);

    }
}
