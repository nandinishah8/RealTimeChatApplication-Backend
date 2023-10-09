namespace MinimalChatApplication.Models
{
    public class ChannelMessage
    {

        public int Id { get; set; }
        public string SenderId { get; set; }
        public int ChannelId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
