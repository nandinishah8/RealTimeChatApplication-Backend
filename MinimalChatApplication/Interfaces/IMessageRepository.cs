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

        //Task MarkMessageAsSeenAsync(Guid messageId, string userId);
        IEnumerable<Message> GetUnreadMessages(string receiverId);
        void MarkMessageAsRead(Message message);

        void SaveChanges();

        //  bool MarkMessagesAsSeen(string currentUserId, string receiverId);
        Dictionary<string, int> GetReadUnreadMessageCounts(string userId);

    }
}
