using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MinimalChatApplication.Models
{
    public class Channels
    {
        [Key]
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        
        public ICollection<ChannelMember> ChannelMembers { get; set; }
    }
}
