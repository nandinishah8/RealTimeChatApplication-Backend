namespace MinimalChatApplication.Services
{
    using MinimalChatApplication.Interfaces;
    using MinimalChatApplication.Models;
    using MinimalChatApplication.Repositories;
    using System.Collections.Generic;
    using System.Threading.Channels;

    public class ChannelsService : IChannelService
    {
        private readonly IChannelRepository _channelsRepository;

        public ChannelsService(IChannelRepository channelRepository)
        {
            _channelsRepository = channelRepository;
        }

        public async Task<Channels> CreateChannelAsync(Channels channel)
        {
            
            var createdChannel = await _channelsRepository.CreateChannelAsync(channel);
            return createdChannel;
        }


        public async Task<Channels> GetChannelAsync(int channelId)
        {

            var channel = await _channelsRepository.GetChannelAsync(channelId);

            if (channel == null)
            {
                throw new InvalidOperationException("Channel not found.");
            }

            return channel;
        }

        public async Task<bool> AddMembersToChannelAsync(int channelId, List<string> memberIds)
        {
            try
            {
                // Convert memberIds to ChannelMember objects
                var membersToAdd = memberIds.Select(memberId => new ChannelMember { UserId = memberId, ChannelId = channelId }).ToList();

                // Call the repository method
                return await _channelsRepository.AddMembersToChannelAsync(channelId, membersToAdd);
            }
            catch (Exception ex)
            {
                // Handle exceptions and log errors
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
                // Handle exceptions and log errors
                throw new Exception("Failed to retrieve members in the channel.", ex);
            }
        }

    }
}
