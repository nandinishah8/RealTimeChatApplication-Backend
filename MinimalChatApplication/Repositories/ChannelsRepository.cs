using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalChatApplication.Data;
using MinimalChatApplication.Interfaces;
using MinimalChatApplication.Migrations;
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

        public async Task UpdateChannelAsync(Channels channel)
        {
            _context.Entry(channel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Channels> GetChannelAsync(int channelId)
        {
           
            return await _context.Channel.FindAsync(channelId);
        }

       
        public async Task<bool> DeleteChannelAsync(Channels channel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                   
                    var channelMessages = _context.Messages.Where(m => m.ChannelId == channel.ChannelId);
                    _context.Messages.RemoveRange(channelMessages);

                  
                    var channelMembers = _context.ChannelMembers.Where(cm => cm.ChannelId == channel.ChannelId);
                    _context.ChannelMembers.RemoveRange(channelMembers);

                  
                    _context.Channel.Remove(channel);

                   
                    _context.SaveChanges();

                    
                    transaction.Commit();

                    return true; 
                }
                catch (Exception ex)
                {
                    
                    transaction.Rollback();

                    throw new Exception("Failed to delete the channel.", ex);
                }
            }
        }



        public async Task<bool> AddMembersToChannelAsync(int channelId, List<ChannelMember> members)
            {

            _context.ChannelMembers.AddRange(members);
            await _context.SaveChangesAsync();
            return true; 
        }


        
        public async Task<List<Channels>> GetChannelsByUserAsync(string userId)
        {
            

            var channels = await _context.ChannelMembers
                .Where(cm => cm.UserId == userId)
                .Select(cm => cm.Channel)
                .ToListAsync();

            return channels;
        }


        public List<Channels> GetChannels()
        {
            return _context.Channel
        .Include(c => c.ChannelMembers) 
        .ToList();
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
                        Name = u.UserName, 
                        Email = u.Email 
                    })
                    .ToListAsync();

                return memberProfiles;
            }
            catch (Exception ex)
            {
                
                throw new Exception("Failed to retrieve member profiles in the channel from the database.", ex);
            }
        }

        public async Task<bool> DeleteMembersFromChannelAsync(int channelId, List<string> memberIds)
        {
            try
            {
                var channelMembersToRemove = await _context.ChannelMembers
                    .Where(cm => cm.ChannelId == channelId && memberIds.Contains(cm.UserId))
                    .ToListAsync();

                _context.ChannelMembers.RemoveRange(channelMembersToRemove);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete members from the channel.", ex);
            }
        }

    }
}
