namespace MinimalChatApplication.Services
{
    using MinimalChatApplication.Interfaces;
    using MinimalChatApplication.Models;
    using MinimalChatApplication.Repositories;
    using NuGet.Protocol.Core.Types;
    using System.Collections.Generic;
    using System.Threading.Channels;

    public class ChannelsService : IChannelService
    {
        private readonly IChannelRepository _channelsRepository;

        public ChannelsService(IChannelRepository channelRepository)
        {
            _channelsRepository = channelRepository;
        }

        
        public async Task<Channels> CreateChannelAsync(string name, string description, string creatorUserId)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(creatorUserId))
            {
               
                throw new ArgumentException("Invalid input data.");
            }

            var channel = new Channels
            {
                Name = name,
                Description = description,
                CreatedAt = DateTime.Now
            };

            try
            {
                
                var createdChannel = await _channelsRepository.CreateChannelAsync(channel);

               
                await _channelsRepository.AddMembersToChannelAsync(createdChannel.ChannelId, new List<ChannelMember> { new ChannelMember { UserId = creatorUserId, ChannelId = createdChannel.ChannelId } });

                return createdChannel;
            }
            catch (Exception ex)
            {
                
                throw new Exception("Failed to create the channel.", ex);
            }
        }

        public async Task<Channels> EditChannelAsync(int channelId, CreateChannelRequest editChannelRequest)
        {
            try
            {
                var channel = await _channelsRepository.GetChannelAsync(channelId);

                if (channel == null)
                {
                    throw new Exception("Channel not found.");
                }

                if (editChannelRequest.Name != null)
                {
                    channel.Name = editChannelRequest.Name;
                }

                if (editChannelRequest.Description != null)
                {
                    channel.Description = editChannelRequest.Description;
                }

                await _channelsRepository.UpdateChannelAsync(channel);

                return channel;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to edit the channel.", ex);
            }
        }




        public async Task<List<Channels>> GetChannelsByUserAsync(string userId)
        {
            try
            {
                
                var channels = await _channelsRepository.GetChannelsByUserAsync(userId);
                return channels;
            }
            catch (Exception ex)
            {
               
                throw new Exception("Failed to retrieve channels by user from the database.", ex);
            }
        }


        public List<Channels> GetChannels()
        {
            return _channelsRepository.GetChannels();
        }

        public async Task<bool> DeleteChannelAsync(int channelId)
        {
            try
            {
                var channel = await _channelsRepository.GetChannelAsync(channelId);

                if (channel == null)
                {
                    throw new Exception("Channel not found.");
                }

               

                bool deleted = await _channelsRepository.DeleteChannelAsync(channel);
                return deleted;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete the channel.", ex);
            }
        }



        public async Task<bool> AddMembersToChannelAsync(int channelId, List<string> memberIds)
        {
            try
            {
                var membersToAdd = memberIds.Select(memberId => new ChannelMember { UserId = memberId, ChannelId = channelId }).ToList();
                return await _channelsRepository.AddMembersToChannelAsync(channelId, membersToAdd);
            }
            catch (Exception ex)
            {
                
                throw new Exception("Failed to add members to the channel.", ex);
            }
        }


        public async Task<List<UserProfile>> GetMembersInChannelAsync(int channelId)
        {
            try
            {
                return await _channelsRepository.GetMembersInChannelAsync(channelId);
            }
            catch (Exception ex)
            {
                
                throw new Exception("Failed to retrieve members in the channel.", ex);
            }
        }

        public Task<bool> DeleteMembersFromChannelAsync(int channelId, List<string> memberIds)
        {
            return _channelsRepository.DeleteMembersFromChannelAsync(channelId, memberIds);
        }

    }
}
