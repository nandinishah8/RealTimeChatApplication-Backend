﻿using System.Collections.Generic;
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
        .Include(c => c.ChannelMembers) // Load channel members
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

    }
}
