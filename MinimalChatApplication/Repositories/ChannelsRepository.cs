using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Models;

namespace MinimalChatApplication.Repositories
{
    public class ChannelRepository : IChannelRepository
    {
        private readonly MinimalChatContext _context;

        public ChannelRepository(MinimalChatContext context)
        {
            _context = context;
        }

        public async Task<Channels> CreateChannelAsync(Channels channel)
        {
            // Implement channel creation logic using the EF context or your data access method
            _context.Channel.Add(channel);
            await _context.SaveChangesAsync();
            return channel;
        }

        //public async Task<bool> AddMembersToChannelAsync(int channelId, List<string> memberIds)
        //{
        //    // Implement logic to add members to a channel in the database
        //    // Example: Find the channel, validate member IDs, and add members
        //    // Remember to use the _context to make changes to the database
        //    return true; // Return true if the operation is successful
        //}

        public async Task<Channels> GetChannelAsync(int channelId)
        {
            // logic to retrieve a channel from the database
            return await _context.Channel.FindAsync(channelId);
        }
    }
}
