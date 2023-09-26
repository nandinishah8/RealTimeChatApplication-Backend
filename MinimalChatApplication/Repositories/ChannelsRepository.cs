using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
           
            _context.Channel.Add(channel);
            await _context.SaveChangesAsync();
            return channel;
        }

        public async Task<bool> AddMembersToChannelAsync(int channelId, List<ChannelMember> members)
        {
            
            _context.ChannelMembers.AddRange(members);
            await _context.SaveChangesAsync();
            return true; // Return true if the operation is successful.
        }


        public async Task<Channels> GetChannelAsync(int channelId)
        {
            
            return await _context.Channel.FindAsync(channelId);
        }
    }
}
