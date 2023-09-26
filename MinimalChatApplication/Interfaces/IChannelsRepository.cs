using System.Security.Cryptography;
using System.Threading.Channels;
using System.Threading.Tasks;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IChannelRepository
    {
        Task<Channels> CreateChannelAsync(Channels channel);
       // Task<bool> AddMembersToChannelAsync(int channelId, ChannelMember channelMember);

        //void AddMembersToChannel(int channelId, List<ChannelMember> members);
        Task<Channels> GetChannelAsync(int channelId);
        Task<bool> AddMembersToChannelAsync(int channelId, List<ChannelMember> membersToAdd);
    }
}
