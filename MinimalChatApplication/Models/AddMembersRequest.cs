namespace MinimalChatApplication.Models
{
    public class AddMembersRequest
    {
        public int ChannelId { get; set; }
        public List<string> UserIds { get; set; }
    }
}
