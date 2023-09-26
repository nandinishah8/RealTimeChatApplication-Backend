using System.Security.Cryptography;
using System.Threading.Channels;
using System.Threading.Tasks;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IChannelRepository
    {
        Task<Channels> CreateChannelAsync(Channels channel);
       
        Task<Channels> GetChannelAsync(int channelId);
        Task<bool> AddMembersToChannelAsync(int channelId, List<ChannelMember> membersToAdd);
        Task<List<UserProfile>> GetMembersInChannelAsync(int channelId);
    }
}
