using System.Security.Cryptography;
using System.Threading.Channels;
using System.Threading.Tasks;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Interfaces
{
    public interface IChannelRepository
    {
        Task<Channels> CreateChannelAsync(Channels channel);

        

        Task<List<Channels>> GetChannelsByUserAsync(string userId);
        List<Channels> GetChannels();
        Task<bool> AddMembersToChannelAsync(int channelId, List<ChannelMember> membersToAdd);
        Task<List<UserProfile>> GetMembersInChannelAsync(int channelId);
    }
}
