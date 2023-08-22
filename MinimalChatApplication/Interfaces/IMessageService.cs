using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IMessageService
    {

        Task<List<Message>> GetConversationHistory(ConversationRequest request, string userId);


        Task<List<Message>> GetMessageHistory(string result);

        Task<sendMessageResponse> SendMessageAsync(sendMessageRequest model, string senderId);

       

        //Task<IActionResult> PutMessage(int id, Message message);

        //Task<IActionResult> DeleteMessage(int id);

    }
}
