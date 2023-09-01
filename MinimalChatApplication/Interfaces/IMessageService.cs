using Microsoft.AspNetCore.Mvc;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IMessageService
    {

        Task<List<Message>> GetConversationHistory(ConversationRequest request, string userId);



        Task<sendMessageResponse> PostMessage(sendMessageRequest model, string senderId);
     



        Task<IActionResult> PutMessage(int id, EditMessage message);

        //Task<IActionResult> DeleteMessage(int id);

    }
}
