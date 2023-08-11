namespace MinimalChatApplication.Interfaces
{
    public interface IMessageRepository
    {
        bool SendMessage(int senderId, int receiverId, string content, out int messageId);
    }
}
