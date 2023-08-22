using System.Collections.Concurrent;

namespace MinimalChatApplication.Models
{
    public class Connection
    {
        private readonly ConcurrentDictionary<string, string> UserConnectionMap = new ConcurrentDictionary<string, string>();

        public void AddConnection(string userId, string connectionId)
        {
            UserConnectionMap[userId] = connectionId;
        }

        public Task<string> GetConnectionIdAsync(string userId)
        {
            if (UserConnectionMap.TryGetValue(userId, out var connectionId))
            {
                return Task.FromResult(connectionId);
            }
            else
            {
                return Task.FromResult<string>(null);
            }

        }
    }
}
