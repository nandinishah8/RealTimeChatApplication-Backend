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
            // Your implementation to create the channel using _channelRepository
            // Example:
            var createdChannel = await _channelsRepository.CreateChannelAsync(channel);
            return createdChannel;
        }

        //public Channel GetChannel(int id)
        //{
        //    return _channelsRepository.GetChannel(id);
        //}

        //public void AddMembersToChannel(List<ChannelMember> members)
        //{
        //    // Add any additional business logic here
        //    _channelsRepository.AddMembersToChannel(members);
        //}
    }
}
