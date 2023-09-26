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

        //public void AddMembersToChannel(List<ChannelMember> members)
        //{
        //    // Add any additional business logic here
        //    _channelsRepository.AddMembersToChannel(members);
        //}
    }
}
