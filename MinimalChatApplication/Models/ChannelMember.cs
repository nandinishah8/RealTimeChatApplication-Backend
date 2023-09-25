using Microsoft.AspNetCore.Identity;

namespace MinimalChatApplication.Models
{
    public class ChannelMember 
    {
        public string UserId { get; set; }
        public int ChannelId { get; set; }

        public IdentityUser User { get; set; }
        public Channels Channel { get; set; }
    }
}
