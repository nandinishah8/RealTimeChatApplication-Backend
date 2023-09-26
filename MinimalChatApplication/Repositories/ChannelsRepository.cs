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

        public async Task<List<UserProfile>> GetMembersInChannelAsync(int channelId)
        {
            try
            {
                var memberProfiles = await _context.ChannelMembers
                    .Where(cm => cm.ChannelId == channelId)
                    .Join(_context.Users, cm => cm.UserId, u => u.Id, (cm, u) => new UserProfile
                    {
                        Id = u.Id,
                        Name = u.UserName, // Replace with your actual property
                        Email = u.Email // Replace with your actual property
                    })
                    .ToListAsync();

                return memberProfiles;
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
                throw new Exception("Failed to retrieve member profiles in the channel from the database.", ex);
            }
        }

    }
}
