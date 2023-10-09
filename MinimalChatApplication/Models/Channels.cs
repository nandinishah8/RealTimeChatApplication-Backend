using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinimalChatApplication.Models
{
    public class Channels
    {
        [Key]
        public int ChannelId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        [JsonIgnore]
        public ICollection<ChannelMember> ChannelMembers { get; set; }

        [JsonIgnore]
        public ICollection<Message> Messages { get; set; }
    }
}
