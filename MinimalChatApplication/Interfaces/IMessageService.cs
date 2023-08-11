using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IMessageService
    {
        sendMessageResponse SendMessage(sendMessageRequest request);
    }
}
