using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IChannelService
    {
        //Task<Channels> CreateChannelAsync(Channels channel);
        Task<Channels> CreateChannelAsync(string name, string description, string creatorUserId);
        Task<bool> AddMembersToChannelAsync(int channelId, List<string> memberIds);
       
        Task<List<Channels>> GetChannelsByUserAsync(string userId);

        List<Channels> GetChannels();

        Task<List<UserProfile>> GetMembersInChannelAsync(int channelId);
    }
}
