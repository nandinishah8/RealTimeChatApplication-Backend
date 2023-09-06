namespace MinimalChatApplication.Models
{
    public class MarkMessageAsSeenDto
    {
        public int MessageId { get; set; }
        public string UserId { get; set; }
        public string CurrentUserId { get;  set; }
        public string ReceiverId { get;  set; }
    }
}
