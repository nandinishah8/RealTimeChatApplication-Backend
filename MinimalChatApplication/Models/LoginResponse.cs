namespace MinimalChatApplication.Models
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public UserProfile Profile { get; set; }
    }
}
