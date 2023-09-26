using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IChannelService
    {
        Task<Channels> CreateChannelAsync(Channels channel);
        //Task<bool> AddMembersToChannelAsync(int channelId, List<string> memberIds);
        //Task<Channel> GetChannelAsync(int channelId);
    }
}
