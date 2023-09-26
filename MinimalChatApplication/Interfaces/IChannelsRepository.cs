using System.Threading.Channels;
using System.Threading.Tasks;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IChannelRepository
    {
        Task<Channels> CreateChannelAsync(Channels channel);
        //Task<bool> AddMembersToChannelAsync(int channelId, ChannelMember channelMember);
        Task<Channels> GetChannelAsync(int channelId);
    }
}
