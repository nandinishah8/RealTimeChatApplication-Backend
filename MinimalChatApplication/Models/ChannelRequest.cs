namespace MinimalChatApplication.Models
{
    public class ChannelRequest
    {
        public int ChannelId { get; set; }
        public DateTime? Before { get; set; }
        public int Count { get; set; } = 20;
        public string Sort { get; set; } = "asc";
    }
}
